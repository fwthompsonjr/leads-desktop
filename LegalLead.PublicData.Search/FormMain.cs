using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{

    public partial class FormMain : Form
    {
        protected WebFetchResult CaseData { get; set; }

        public FormMain()
        {
            InitializeComponent();
            // set application title
            var appName = Assembly.GetExecutingAssembly().GetName();
            string version = appName.Version.ToString();
            Text = string.Format(@"{0} - {1}",
                appName.Name, version);

            BindComboBoxes();
            SetDentonStatusLabelFromSetting();
            SetStatus(StatusType.Ready);
        }

        private void SetStatus(StatusType status)
        {
            var v = StatusHelper.GetStatus(status);
            toolStripStatus.Text = string.Format("{0}", v.Name);
            toolStripStatus.ForeColor = v.Color;
            Refresh();
            Application.DoEvents();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private void Button1_Click(object sender, EventArgs e)
        {
            
            try
            {
                KillProcess("chromedriver");
                SetStatus(StatusType.Running);
                if (!ValidateCustom())
                {
                    SetStatus(StatusType.Ready);
                    return;
                }
                txConsole.Text = string.Empty;
                ProcessStartingMessage();
                var startDate = dteStart.Value.Date;
                var endingDate = dteEnding.Value.Date;
                var siteData = (WebNavigationParameter)(cboWebsite.SelectedItem);

                const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
                var isDentonCounty = siteData.Id == (int)SourceType.DentonCounty;
                var keys = siteData.Keys;
                var isDistrictSearch = keys.FirstOrDefault(x => x.Name.Equals("DistrictSearchType", ccic)) != null;
                var criminalToggle = keys.FirstOrDefault(x => x.Name.Equals("criminalCaseInclusion", ccic));
                if (isDentonCounty && criminalToggle != null)
                {
                    criminalToggle.Value = isDistrictSearch ? "0" : "1";
                }

                if(!isDentonCounty & criminalToggle != null)
                {
                    criminalToggle.Value = "1";
                }

                IWebInteractive webmgr =
                    WebFetchingProvider.
                    GetInteractive(siteData, startDate, endingDate);

                CaseData = webmgr.Fetch();

                ProcessEndingMessage();
                SetStatus(StatusType.Finished);
                KillProcess("chromedriver");
                if (CaseData == null)
                {
                    throw new ApplicationException("No data found from case extract.");
                }
                if (string.IsNullOrEmpty(CaseData.Result))
                {
                    throw new ApplicationException("No data found from case extract.");
                }
                CaseData.WebsiteId = siteData.Id;
                ExcelWriter.WriteToExcel(CaseData);

                var result = MessageBox.Show(
                    "Your data extract has completed. Would you like to view in Excel?",
                    "Data extract success",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result != System.Windows.Forms.DialogResult.Yes)
                {
                    SetStatus(StatusType.Ready);
                    return;
                }
                TryOpenExcel();
                SetStatus(StatusType.Ready);
            }
            catch (Exception ex)
            {
                SetStatus(StatusType.Error);
                Console.WriteLine("An unexpected error occurred.");
                Console.WriteLine(ex.Message);

                Console.WriteLine("- - - - - - - - - - - - - - - - -");
                Console.WriteLine(ex.StackTrace);
            }
        }



        private void exportDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CaseData == null)
            {
                ShowNoDataErrorBox();
                return;
            }
            if (string.IsNullOrEmpty(CaseData.Result))
            {
                ShowNoDataErrorBox();
                return;
            }
        }

        private void ShowNoDataErrorBox()
        {
            MessageBox.Show("Please check data extract. No source data has been found for export.",
                "Data Not Found",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void ButtonDentonSetting_Click(object sender, EventArgs e)
        {

            var sourceId = ((WebNavigationParameter)cboWebsite.SelectedItem).Id;
            if(sourceId == (int)SourceType.DentonCounty)
            {
                using (var credential = new FormCredential())
                {
                    credential.ShowDialog(this);
                    SetDentonStatusLabelFromSetting();
                }
            }
            else
            {
                using (var result = new FormCredential())
                {
                    result.ShowDialog(this); 
                }
            }
        }

        private void KillProcess(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }
    }
}
