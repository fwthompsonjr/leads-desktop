using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    using winforms = System.Windows.Forms;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<Pending>")]
    public partial class FormMain : Form
    {
        #region Private Members

        private readonly string SubmitButtonText;

        #endregion

        #region Custom Properties

        protected WebFetchResult CaseData { get; set; }
        private FindDbRequest CurrentRequest { get; set; } = null;

        #endregion

        #region Constructor

        public FormMain()
        {
            InitializeComponent();
            // set application title
            var appName = Assembly.GetExecutingAssembly().GetName();
            string version = appName.Version.ToString();
            Text = string.Format(CultureInfo.CurrentCulture, @"{0} - {1}",
                appName.Name, version);
            SubmitButtonText = button1.Text;
            FormClosing += FormMain_FormClosing;
            Shown += FormMain_Shown;
            BindComboBoxes();
            SetDentonStatusLabelFromSetting();
            SetStatus(StatusType.Ready);
        }

        #endregion

        #region Form Event Handlers

        private void FormMain_Shown(object sender, EventArgs e)
        {
            SetInteraction(false);
            var form = Program.loginForm;
            var rsp = form.DialogResult;
            if (rsp == DialogResult.None) rsp = form.ShowDialog();
            switch (rsp)
            {
                case DialogResult.OK:
                case DialogResult.Yes:
                    Debug.WriteLine("Login success");
                    FilterWebSiteByAccount();
                    SetInteraction(true);
                    UsageReader.WriteUserRecord();
                    ApplySavedSettings();
                    CboWebsite_SelectedValueChanged(null, null);
                    break;
                default:
                    Close();
                    break;
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            const string processNames = "chromedriver,geckodriver";
            KillProcess(processNames);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // method is intentionally left blank
        }

        private void ExportDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CaseData == null)
            {
                ShowNoDataErrorBox();
                return;
            }
            if (string.IsNullOrEmpty(CaseData.Result))
            {
                ShowNoDataErrorBox();
            }
        }

        private void ButtonDentonSetting_Click(object sender, EventArgs e)
        {

            var sourceId = ((WebNavigationParameter)cboWebsite.SelectedItem).Id;
            switch (sourceId)
            {
                case (int)SourceType.DentonCounty:
                    using (var credential = new FormDentonSetting())
                    {
                        credential.Icon = Icon;
                        credential.ShowDialog(this);
                        SetDentonStatusLabelFromSetting();
                    }
                    break;
                default:
                    using (var result = new FormCredential())
                    {
                        result.Icon = Icon;
                        result.ShowDialog(this);
                    }
                    break;
            }
        }

        private void ToolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            var settings = new FormSettings() { StartPosition = FormStartPosition.CenterParent };
            settings.ShowDialog();
        }

        #endregion

        #region Private Helper Methods

        private void SetUserName()
        {
            var username = string.Empty;
            try
            {
                username = GetUserName();
                if (string.IsNullOrEmpty(username)) return;
            }
            finally
            {
                var isVisible = !string.IsNullOrEmpty(username);
                var txt = isVisible ? $" | {username}" : string.Empty;
                tsUserName.Text = txt;
                tsUserName.Visible = isVisible;
            }
        }

        private void SetInteraction(bool isEnabled)
        {
            if (!isEnabled) TryHideProgress();
            cboWebsite.Enabled = isEnabled;
            button1.Enabled = isEnabled;
            tsSettingMenuButton.Visible = isEnabled;
            dteEnding.Enabled = isEnabled;
            dteStart.Enabled = isEnabled;
        }

        private void FilterWebSiteByAccount()
        {
            try
            {
                var webdetail = GetAccountIndexes();
                SetUserName();
                if (string.IsNullOrEmpty(webdetail))
                {
                    // remove all websites from cboWebsite
                    // and disable the controls
                    SetInteraction(false);
                    cboWebsite.Items.Clear();
                    return;
                }
                if (webdetail.Equals("-1")) return;
                var webid = webdetail.Split(',')
                    .Where(w => { return int.TryParse(w, out var _); })
                    .Select(s => int.Parse(s, CultureInfo.CurrentCulture))
                    .ToList();
                if (!webid.Any())
                {
                    // remove all websites from cboWebsite
                    // and disable the controls
                    SetInteraction(false);
                    cboWebsite.Items.Clear();
                    return;
                }

                var websites = SettingsManager.GetNavigation()
                    .FindAll(x => webid.Contains(x.Id));
                cboWebsite.SelectedValueChanged -= CboWebsite_SelectedValueChanged;
                cboWebsite.DataSource = websites;
                cboWebsite.DisplayMember = CommonKeyIndexes.NameProperCase;
                cboWebsite.ValueMember = CommonKeyIndexes.IdProperCase;
                cboWebsite.SelectedValueChanged += CboWebsite_SelectedValueChanged;
                cboWebsite.SelectedIndex = 0;
            }
            finally
            {
                SortWebsites();
            }
        }

        private void SetStatus(StatusType status)
        {
            var v = StatusHelper.GetStatus(status);
            try
            {
                toolStripStatus.Text = string.Format(CultureInfo.CurrentCulture, "{0}", v.Name);
                toolStripStatus.ForeColor = v.Color;
                var enabled = status != StatusType.Running;
                SetControlEnabledState(enabled);
                Refresh();
            }
            catch (Exception)
            {
                SetStatusFromOffThread(v);
            }
            Application.DoEvents();
        }

        private void SetControlEnabledState(bool isEnabled)
        {
            // need to only toggle button-submit etc.
            // when it has not been disabled by a different rule
            var collection = tableLayoutPanel1.Controls;
            foreach (Control control in collection)
            {
                if (control.Visible && control is winforms.ComboBox cx) { cx.Enabled = isEnabled; }
                if (control.Visible && control is winforms.Button btn) { btn.Enabled = isEnabled; }
                if (control.Visible && control is DateTimePicker dpicker) { dpicker.Enabled = isEnabled; }
            }
        }

        private void SetStatusFromOffThread(StatusState v, StatusType status = StatusType.None)
        {
            this.Invoke(new Action(() =>
            {
                toolStripStatus.Text = string.Format(CultureInfo.CurrentCulture, "{0}", v.Name);
                toolStripStatus.ForeColor = v.Color;
                var enabled = status != StatusType.Running;
                SetControlEnabledState(enabled);
                Refresh();
            }));
        }

        private void SortWebsites()
        {
            if (cboWebsite.DataSource is not List<WebNavigationParameter> dlist) return;
            if (dlist.Count == 0) return;
            dlist.Sort((a, b) => { return a.Name.CompareTo(b.Name); });
            var itemIndex = cboWebsite.SelectedItem is not WebNavigationParameter item ? 0 : dlist.IndexOf(item);
            cboWebsite.SelectedValueChanged -= CboWebsite_SelectedValueChanged;
            cboWebsite.DataSource = null;
            cboWebsite.DataSource = dlist;
            cboWebsite.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboWebsite.ValueMember = CommonKeyIndexes.IdProperCase;
            cboWebsite.SelectedValueChanged += CboWebsite_SelectedValueChanged;
            cboWebsite.SelectedIndex = itemIndex;
        }


        #endregion

        #region Static Methods

        protected static T GetObject<T>(object item)
        {
            return (T)item;
        }

        private static int GetCaseSelectionIndex(object selectedItem)
        {
            var fallback = 0;
            if (selectedItem == null) return fallback;
            if (selectedItem is not Option ddp) return fallback;
            if (!ddp.Name.Contains("criminal")) return fallback;
            return 1;
        }

        private static void ShowNoDataErrorBox()
        {
            MessageBox.Show(CommonKeyIndexes.PleaseCheckSourceDataNotFound,
                CommonKeyIndexes.DataNotFound,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static string GetUserName()
        {
            var container = AuthenicationContainer.GetContainer;
            var userservice = container.GetInstance<SessionUserPersistence>();
            return userservice.GetUserName();
        }
        private static bool IsAccountAdmin()
        {
            return GetAccountIndexes().Equals("-1");
        }

        private static string GetAccountIndexes()
        {
            var container = SessionPersistenceContainer.GetContainer;
            var instance = container.GetInstance<ISessionPersistance>(ApiHelper.ApiMode);
            return instance.GetAccountPermissions();
        }

        private static void KillProcess(string processName)
        {
            var processes = new List<string> { processName };
            if (processName.Contains(','))
            {
                processes = processName.Split(',').ToList();
            }
            processes.ForEach(Kill);
        }

        private static void Kill(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }

        private static bool IsEmpty(WebFetchResult caseData)
        {
            if (caseData == null) return true;
            const string emptyCases = "<table></table>";
            return caseData.CaseList.Equals(emptyCases, StringComparison.OrdinalIgnoreCase);
        }


        #endregion

        #region Private Static Fields

        private static readonly SessionUsagePersistence UsageIncrementer
            = SessionPersistenceContainer.GetContainer
            .GetInstance<SessionUsagePersistence>();
        private static readonly SessionMonthToDatePersistence UsageReader
            = SessionPersistenceContainer.GetContainer
            .GetInstance<SessionMonthToDatePersistence>();


        #endregion

        #region Classes

        private static class ControlExtensions
        {
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool LockWindowUpdate(IntPtr hWndLock);

            public static void Suspend(Control control)
            {
                LockWindowUpdate(control.Handle);
            }

            public static void Resume()
            {
                LockWindowUpdate(IntPtr.Zero);
            }

        }

        #endregion
    }
}
