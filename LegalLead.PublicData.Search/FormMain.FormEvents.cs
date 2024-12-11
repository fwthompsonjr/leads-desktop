using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FormMain : Form
    {

        internal void SetDentonStatusLabelFromSetting()
        {
            const int One = 1;
            var srcId = ((WebNavigationParameter)cboWebsite.SelectedItem).Id;
            if (srcId != (int)SourceType.DentonCounty)
            {
                return;
            }
            var sb = new StringBuilder();
            var searchDto = SearchSettingDto.GetDto();
            var showDistrict = searchDto.CountySearchTypeId == (int)SourceType.DentonCounty;
            var courtNames = CaseTypeSelectionDto.GetDto(
                showDistrict ?
                CommonKeyIndexes.DentonDistrictCaseType :
                CommonKeyIndexes.DentonCountyCaseType);
            sb.AppendFormat(
                CultureInfo.CurrentCulture,
                CommonKeyIndexes.SearchColonSpaceElement, showDistrict ?
                CommonKeyIndexes.DistrictCourts :
                CommonKeyIndexes.JpCountyCourts);

            var subItemId = showDistrict ?
                searchDto.DistrictSearchTypeId :
                searchDto.CountySearchTypeId;

            var courtItemId = showDistrict ?
                searchDto.DistrictCourtId + One :
                searchDto.CountyCourtId + One;

            var courtDropDown = courtNames.DropDowns[0];
            var nameDropDown = courtDropDown.Options.Find(x => x.Id == courtItemId);

            _ = sb.AppendFormat(
                CultureInfo.CurrentCulture,
                CommonKeyIndexes.SpaceDashSpaceElement, nameDropDown.Name);
            if (showDistrict)
            {
                _ = sb.AppendFormat(
                CultureInfo.CurrentCulture,
                CommonKeyIndexes.SpaceDashSpaceElement,
                CommonKeyIndexes.CriminalCivilAndFamily.Split(',')[subItemId]);
            }
            tsStatusLabel.Text = sb.ToString();
        }

        private void CboWebsite_SelectedValueChanged(object sender, EventArgs e)
        {
            var changeProvider = new WebsiteChangeProvider(this);
            var changeHandler = changeProvider.GetProvider();
            if (changeHandler == null)
            {
                return;
            }

            changeHandler.Change();
            DentonSettingHandler();
            CustomNoteHandler();
            WebSiteUsageValidation();
            CustomWebsiteChangeHandler();
        }
        private void DentonSettingHandler()
        {
            if (!ButtonDentonSetting.Visible) return;
            if (cboWebsite.SelectedItem is not WebNavigationParameter webitem) return;
            if (webitem.Id == (int)SourceType.DentonCounty) return;
            ButtonDentonSetting.Visible = false;
        }
        private void CustomNoteHandler()
        {
            var messages = CustomCountyDto.GetNotes();
            var style = tableLayoutPanel1.RowStyles[9];
            if (cboWebsite.SelectedItem is not WebNavigationParameter webitem ||
                !messages.Exists(x => x.Id == webitem.Id))
            {
                // hide panel
                style.SizeType = SizeType.Absolute;
                style.Height = 0;
                label5.Visible = false;
                lbNotes.Visible = false;
                lbNotes.Text = string.Empty;
                return;
            }
            var item = messages.Find(x => x.Id == webitem.Id);
            style.SizeType = SizeType.Absolute;
            style.Height = 40;
            label5.Visible = true;
            lbNotes.Visible = true;
            lbNotes.Text = string.Join(Environment.NewLine, item.Notes);
        }
        private void CustomWebsiteChangeHandler()
        {
            if (cboWebsite.SelectedItem is not WebNavigationParameter nav) return;
            var collinId = (int)SourceType.CollinCounty;
            var harrisId = (int)SourceType.HarrisCivil;
            if (nav.Id == collinId)
            {
                CboSearchType_SelectedIndexChanged(this, EventArgs.Empty);
                return;
            }
            if (nav.Id != harrisId) return;
            if (cboCaseType.DataSource is not List<Option> list) return;
            var db = new List<Option>();
            var items = list.FindAll(x =>
            {
                if (x.Name.Contains("criminal")) return true;
                if (x.Name.Contains("civil")) return true;
                return false;
            });
            db.AddRange(items);
            cboCaseType.DataSource = db;
            cboCaseType.Refresh();
            cboCaseType.SelectedIndex = db.Count - 1;
            cboCaseType.Visible = true;
        }

        private void WebSiteUsageValidation()
        {
            var isEnabled = true;
            var limit = 0;
            var monthtodate = 0;
            var controlled = Enum.GetValues<SourceType>().Select(s => (int)s).ToList();
            if (cboWebsite.SelectedItem is not WebNavigationParameter nav) return;
            if (!controlled.Contains(nav.Id)) return;
            var member = ((SourceType)nav.Id).GetCountyName();
            try
            {
                var userinfo = SessionUtil.Read();
                if (string.IsNullOrEmpty(userinfo)) return;
                var user = userinfo.ToInstance<LeadUserSecurityBo>();
                if (user == null) return;
                limit = UsageGovernance.GetUsageLimit(nav.Id);
                if (limit == -1) return;
                monthtodate = UsageReader.GetCount(member);
                if (monthtodate < limit) return;
                isEnabled = false;
            }
            finally
            {
                button1.Enabled = isEnabled;
                var contact = "Contact administrator for further details.";
                var usagemessage = $"Monthly usage exceeded : {monthtodate} of {limit}.";
                var subtext = monthtodate == 0 || limit == 0 ? contact : usagemessage;
                var alternate = $"{member} Search disabled. {subtext}";
                var txt = isEnabled ? SubmitButtonText : alternate;
                button1.Text = txt;
                if (ButtonDentonSetting.Visible) { ButtonDentonSetting.Enabled = isEnabled; }
            }

        }

        internal void CboSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var source = (DropDown)cboSearchType.SelectedItem;
            var nonactors = new List<int> {
                (int)SourceType.HarrisCivil,
                (int)SourceType.DallasCounty,
                (int)SourceType.TravisCounty,
                (int)SourceType.BexarCounty,
                (int)SourceType.HidalgoCounty,
                (int)SourceType.ElPasoCounty,
                (int)SourceType.FortBendCounty,
                (int)SourceType.WilliamsonCounty,
                (int)SourceType.GraysonCounty,
            };
            var selectedItem = (WebNavigationParameter)cboWebsite.SelectedItem;
            if (selectedItem != null && nonactors.Contains(selectedItem.Id))
            {
                return;
            }

            var caseTypes = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.CollinCountyCaseType);
            var caseIndex = cboSearchType.SelectedIndex;

            // remove event handler
            cboSearchType.SelectedIndexChanged -= CboSearchType_SelectedIndexChanged;

            cboSearchType.DataSource = caseTypes.DropDowns;
            cboSearchType.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboSearchType.ValueMember = CommonKeyIndexes.IdProperCase;
            cboSearchType.SelectedIndex = caseIndex;

            // restore event handler
            cboSearchType.SelectedIndexChanged += CboSearchType_SelectedIndexChanged;

            cboCaseType.DataSource = source.Options;
            cboCaseType.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboCaseType.ValueMember = CommonKeyIndexes.IdProperCase;
            cboCaseType.SelectedIndex = 0;
        }

        private static System.Timers.Timer aTimer;

        private static void SetUpTimer()
        {

            // Create a timer and set a two second interval.
            aTimer = new System.Timers.Timer
            {
                Interval = 450
            };

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Enabled = true;
        }

        private void BindComboBoxes()
        {

            var websites = SettingsManager.GetNavigation();
            var hcconfig = HccConfiguration.Load().Dropdown;
            var hcitem = websites.Find(x => x.Id.Equals(hcconfig.Index));
            if (hcitem != null && !hcconfig.IsEnabled) { websites.Remove(hcitem); }
            var caseTypes = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.CollinCountyCaseType);
            var tarrantCourt = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.TarrantCountyCaseType);
            const int Zero = 0;
            // previous files list
            Tag = new List<SearchResult>();
            tsDropFileList.Enabled = false;
            tsDropFileList.Visible = false;

            TsWebDriver_Initialize();

            cboWebsite.DataSource = websites;
            cboWebsite.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboWebsite.ValueMember = CommonKeyIndexes.IdProperCase;

            cboSearchType.Visible = false;
            cboSearchType.DataSource = caseTypes.DropDowns;
            cboSearchType.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboSearchType.ValueMember = CommonKeyIndexes.IdProperCase;


            cboCourts.Visible = false;
            cboCourts.DataSource = tarrantCourt.DropDowns[0].Options;
            cboCourts.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboCourts.ValueMember = CommonKeyIndexes.IdProperCase;

            cboCaseType.Visible = false;
            cboCaseType.DataSource = caseTypes.DropDowns[0].Options;
            cboCaseType.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboCaseType.ValueMember = CommonKeyIndexes.IdProperCase;

            for (int i = 3; i <= 5; i++)
            {
                tableLayoutPanel1.RowStyles[i].SizeType = SizeType.Absolute;
                tableLayoutPanel1.RowStyles[i].Height = Zero;
            }

            cboSearchType.SelectedIndexChanged += CboSearchType_SelectedIndexChanged;
            cboWebsite.SelectedValueChanged += CboWebsite_SelectedValueChanged;


            cboSearchType.SelectedIndex = Zero;
            cboCourts.SelectedIndex = Zero;
            var menuOptions = SessionUtil.GetMenuOptions;
            var dropDownItem = tsSettingMenuButton.DropDownItems;
            dropDownItem.Clear();
            menuOptions.ForEach(item =>
            {
                var child = new ToolStripMenuItem { Tag = item, Text = item.Name };
                child.Click += Child_Click;
                dropDownItem.Add(child);
            });
#if DEBUG
            DebugFormLoad();
#endif
            SetUpTimer();
        }

        private void Child_Click(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem menuitem) return;
            if (menuitem.Tag is not SettingMenuModel model) return;
            var isFormOpen = false;
            foreach (var form in Application.OpenForms)
            {
                if (form is not FormSettings settings) continue;
                isFormOpen = true;
                settings.SetMenuIndex(model.Id);
                settings.Focus();
            }
            if (isFormOpen) return;
            var fs = new FormSettings(model.Id)
            {
                StartPosition = FormStartPosition.CenterParent
            };
            fs.ShowDialog();
        }

        private void ComboBox_DataSourceChanged(object sender, EventArgs e)
        {
            // when data source is changed?
            // remove all items from the tab strip
            try
            {
                tsDropFileList.DropDownItems.Clear();
                var list = GetObject<List<SearchResult>>(Tag);
                list.ForEach(x =>
                {
                    var button = new ToolStripMenuItem
                    {
                        Visible = true,
                        Tag = x,
                        Text = x.Search,
                        DisplayStyle = ToolStripItemDisplayStyle.Text
                    };
                    button.Click += Button_Click;
                    tsDropFileList.DropDownItems.Add(button);
                });

                tsDropFileList.Enabled = list.Count > 0;
                tsDropFileList.Visible = tsDropFileList.Enabled;
            }
            catch (Exception)
            {
                ComboBox_DataSourceChanged_Background();
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            var item = GetObject<SearchResult>(((ToolStripMenuItem)sender).Tag);
            var fileName = item.ResultFileName;
            OpenExcel(ref fileName);
        }

        private void TsWebDriver_Initialize()
        {
            // when data source is changed?
            // remove all items from the tab strip
            tsWebDriver.DropDownItems.Clear();
            var dto = new WebDriverDto().Get();
            var drivers = dto.WebDrivers.Drivers.ToList().FindAll(f => f.IsVerified);
            tsWebDriver.Tag = dto;
            drivers.ForEach(x =>
            {
                var button = new ToolStripMenuItem
                {
                    Visible = true,
                    Tag = x,
                    Text = x.Name,
                    DisplayStyle = ToolStripItemDisplayStyle.Text,
                    Checked = x.Id == dto.WebDrivers.SelectedIndex
                };
                button.Click += WebDriver_Click;
                tsWebDriver.DropDownItems.Add(button);
            });

            tsWebDriver.Enabled = true;
            tsWebDriver.Visible = drivers.Count > 1;
        }



        private void WebDriver_Click(object sender, EventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            var tsItem = (ToolStripMenuItem)sender;
            if (tsItem.Checked)
            {
                return;
            }

            var item = GetObject<Driver>(tsItem.Tag);
            var webItem = GetObject<WebDriverDto>(tsWebDriver.Tag);
            webItem.WebDrivers.SelectedIndex = item.Id;
            webItem.Save();
            // uncheck all
            for (int i = 0; i < tsWebDriver.DropDownItems.Count; i++)
            {
                var menuItem = (ToolStripMenuItem)tsWebDriver.DropDownItems[i];
                var id = GetObject<Driver>(menuItem.Tag).Id;
                menuItem.Checked = id == item.Id;
            }
        }
#if DEBUG
        private void DebugFormLoad()
        {

            // change selected index based upon appSetting
            var configIndex = ConfigurationManager.AppSettings[CommonKeyIndexes.FormContextId];
            var startDate = ConfigurationManager.AppSettings[CommonKeyIndexes.FormStartDate];
            var endDate = ConfigurationManager.AppSettings[CommonKeyIndexes.FormEndDate];
            if (!string.IsNullOrEmpty(configIndex))
            {
                var cfidx = Convert.ToInt32(
                    configIndex,
                    CultureInfo.CurrentCulture);
                if (cfidx < cboWebsite.Items.Count)
                {
                    cboWebsite.SelectedIndex = cfidx;
                    CboWebsite_SelectedValueChanged(null, null);
                }
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                dteStart.Value = DateTime.Parse(startDate, CultureInfo.CurrentCulture.DateTimeFormat);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                dteEnding.Value = DateTime.Parse(endDate, CultureInfo.CurrentCulture.DateTimeFormat);
            }
        }
#endif
        private static void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            Application.DoEvents();
        }


        protected void ProcessStartingMessage()
        {
            var source = (WebNavigationParameter)cboWebsite.SelectedItem;

            var strStarting = dteStart.Value.Date.ToShortDateString();
            var strEnding = dteEnding.Value.Date.ToShortDateString();
            WriteStartSettings(source.Name, strStarting, strEnding);
            var message = CommonKeyIndexes.StartingFetchRequest
                + Environment.NewLine + " " +
                CommonKeyIndexes.WebsiteLabel + source.Name
                + Environment.NewLine + " " +
                CommonKeyIndexes.StartDateLabel + strStarting
                + Environment.NewLine + " " +
                CommonKeyIndexes.EndDateLabel + strEnding
                + Environment.NewLine +
                CommonKeyIndexes.StartTime + DateTime.Now.ToString(
                    CommonKeyIndexes.GeneralLongDate,
                    CultureInfo.CurrentCulture)
                + Environment.NewLine +
                CommonKeyIndexes.DashedLine;

            Console.WriteLine(message);

        }

        protected void ProcessEndingMessage()
        {
            var source = GetParameter();
            var message = GetMessage(source);

            Console.WriteLine(message);

        }

        private void TryOpenExcel()
        {
            var xmlFile = CaseData == null ? string.Empty : CaseData.Result;
            xmlFile = xmlFile.Replace(CommonKeyIndexes.ExtensionXml, CommonKeyIndexes.ExtensionXlsx);
            var movedFile = CommonFolderHelper.MoveToCommon(xmlFile);
            if (!string.IsNullOrEmpty(movedFile) && File.Exists(movedFile)) xmlFile = movedFile;
            OpenExcel(ref xmlFile);
        }

        private static void OpenExcel(ref string xmlFile)
        {
            if (string.IsNullOrEmpty(xmlFile))
            {
                MessageBox.Show(
                    CommonKeyIndexes.FileNotFoundError, // "File not found error.", 
                    CommonKeyIndexes.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
                return;
            }
            if (!File.Exists(xmlFile))
            {
                MessageBox.Show(
                    CommonKeyIndexes.DataSourceNotFoundError, // "Data source file not found error.", 
                    CommonKeyIndexes.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
            xmlFile = xmlFile.Replace(CommonKeyIndexes.ExtensionXml, CommonKeyIndexes.ExtensionXlsx);
            var length = GetFileLength(xmlFile);
            if (length == 0)
            {
                MessageBox.Show(
                    CommonKeyIndexes.ExcelSourceNotFoundError, // "Excel source file not found error.", 
                    CommonKeyIndexes.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            var movedFile = CommonFolderHelper.MoveToCommon(xmlFile);
            if (!string.IsNullOrEmpty(movedFile) && File.Exists(movedFile)) xmlFile = movedFile;
            using var p = new Process();
            p.StartInfo = new ProcessStartInfo(xmlFile)
            {
                UseShellExecute = true
            };
            p.Start();
        }

        private WebNavigationParameter GetParameter()
        {
            try
            {
                return (WebNavigationParameter)cboWebsite.SelectedItem;
            }
            catch (Exception)
            {
                return Invoke(new Func<WebNavigationParameter>(() =>
                    { return (WebNavigationParameter)cboWebsite.SelectedItem; }));
            }
        }
        private static long GetFileLength(string path)
        {
            if (string.IsNullOrEmpty(path)) return 0;
            if (!File.Exists(path)) return 0;
            return new FileInfo(path).Length;
        }
        private string GetMessage(WebNavigationParameter source)
        {
            try
            {
                return CommonKeyIndexes.EndingFetchRequest
                + Environment.NewLine + " " +
                CommonKeyIndexes.WebsiteLabel + source.Name
                + Environment.NewLine + " " +
                CommonKeyIndexes.StartDateLabel + dteStart.Value.Date.ToShortDateString()
                + Environment.NewLine + " " +
                CommonKeyIndexes.EndDateLabel + dteEnding.Value.Date.ToShortDateString()
                + Environment.NewLine +
                CommonKeyIndexes.EndTime + DateTime.Now.ToString(
                    CommonKeyIndexes.GeneralLongDate,
                    CultureInfo.CurrentCulture)
                + Environment.NewLine +
                CommonKeyIndexes.DashedLine
                + Environment.NewLine +
                CommonKeyIndexes.DashedLine;
            }
            catch
            {
                return Invoke(new Func<string>(() =>
                {
                    return CommonKeyIndexes.EndingFetchRequest
                    + Environment.NewLine + " " +
                    CommonKeyIndexes.WebsiteLabel + source.Name
                    + Environment.NewLine + " " +
                    CommonKeyIndexes.StartDateLabel + dteStart.Value.Date.ToShortDateString()
                    + Environment.NewLine + " " +
                    CommonKeyIndexes.EndDateLabel + dteEnding.Value.Date.ToShortDateString()
                    + Environment.NewLine +
                    CommonKeyIndexes.EndTime + DateTime.Now.ToString(
                        CommonKeyIndexes.GeneralLongDate,
                        CultureInfo.CurrentCulture)
                    + Environment.NewLine +
                    CommonKeyIndexes.DashedLine
                    + Environment.NewLine +
                    CommonKeyIndexes.DashedLine;
                }));
            }
        }

        private void ComboBox_DataSourceChanged_Background()
        {
            this.Invoke(new Action(() =>
            {
                // when data source is changed?
                // remove all items from the tab strip
                tsDropFileList.DropDownItems.Clear();
                var list = GetObject<List<SearchResult>>(Tag);
                list.ForEach(x =>
                {
                    var button = new ToolStripMenuItem
                    {
                        Visible = true,
                        Tag = x,
                        Text = x.Search,
                        DisplayStyle = ToolStripItemDisplayStyle.Text
                    };
                    button.Click += Button_Click;
                    tsDropFileList.DropDownItems.Add(button);
                });

                tsDropFileList.Enabled = list.Count > 0;
                tsDropFileList.Visible = tsDropFileList.Enabled;
            }));
        }

        private static void WriteStartSettings(string siteName, string startDate, string endDate)
        {
            const string settingName = "search";
            var settings = new List<UserSettingChangeViewModel>()
            {
                new(){ Category = settingName, Name = "Last County:", Value = siteName },
                new(){ Category = settingName, Name = "Start Date:", Value = startDate },
                new(){ Category = settingName, Name = "End Date:", Value = endDate },
            };
            settings.ForEach(x => { SettingsWriter.Change(x); });
        }

        private static readonly SessionSettingPersistence SettingsWriter =
            SessionPersistenceContainer
            .GetContainer
            .GetInstance<SessionSettingPersistence>();

        private static readonly SessionApiFilePersistence UsageGovernance
            = SessionPersistenceContainer
            .GetContainer
            .GetInstance<SessionApiFilePersistence>();
        // private static readonly string customMessages = 

    }
}
