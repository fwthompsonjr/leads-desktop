using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace LegalLead.PublicData.Search
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<Pending>")]
    public partial class FormMain : Form
    {
        protected WebFetchResult CaseData { get; set; }

        public FormMain()
        {
            InitializeComponent();
            // set application title
            var appName = Assembly.GetExecutingAssembly().GetName();
            string version = appName.Version.ToString();
            Text = string.Format(CultureInfo.CurrentCulture, @"{0} - {1}",
                appName.Name, version);
            FormClosing += FormMain_FormClosing;
            Shown += FormMain_Shown;
            BindComboBoxes();
            SetDentonStatusLabelFromSetting();
            SetStatus(StatusType.Ready);
        }

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
                    SetInteraction(true);
                    FilterWebSiteByAccount();
                    UsageReader.WriteUserRecord();
                    CboWebsite_SelectedValueChanged(null, null);
                    break;
                default:
                    Close();
                    break;
            }
        }

        private void FilterWebSiteByAccount()
        {
            var container = SessionPersistenceContainer.GetContainer;
            var instance = container.GetInstance<ISessionPersistance>(ApiHelper.ApiMode);
            var webdetail = instance.GetAccountPermissions();
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

        private static string GetUserName()
        {
            var container = AuthenicationContainer.GetContainer;
            var userservice = container.GetInstance<SessionUserPersistence>();
            return userservice.GetUserName();
        }
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
            cboWebsite.Enabled = isEnabled;
            button1.Enabled = isEnabled;
            dteEnding.Enabled = isEnabled;
            dteStart.Enabled = isEnabled;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            const string processNames = "chromedriver,geckodriver";
            KillProcess(processNames);
        }

        protected static T GetObject<T>(object item)
        {
            return (T)item;
        }
        private void SetStatus(StatusType status)
        {
            var v = StatusHelper.GetStatus(status);
            try
            {
                toolStripStatus.Text = string.Format(CultureInfo.CurrentCulture, "{0}", v.Name);
                toolStripStatus.ForeColor = v.Color;
                Refresh();
            }
            catch (Exception)
            {
                SetStatusFromOffThread(v);
            }
            Application.DoEvents();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string driverNames = string.Concat("geckodriver,", CommonKeyIndexes.ChromeDriver);
            try
            {
                KillProcess(driverNames);
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
                var searchItem = new SearchResult
                {
                    Id = GetObject<List<SearchResult>>(Tag).Count + 1,
                    Website = siteData.Name,
                    EndDate = endingDate.ToShortDateString(),
                    StartDate = startDate.ToShortDateString(),
                    SearchDate = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString(),
                };
                searchItem.Search = $"{searchItem.SearchDate} : {searchItem.Website} from {searchItem.StartDate} to {searchItem.EndDate}";
                switch (siteData.Id)
                {
                    case (int)SourceType.BexarCounty:
                        BexarButtonExecution(siteData, searchItem);
                        break;
                    case (int)SourceType.DallasCounty:
                        DallasButtonExecution(siteData, searchItem);
                        break;
                    case (int)SourceType.TravisCounty:
                        TravisButtonExecution(siteData, searchItem, startDate, endingDate);
                        break;
                    case (int)SourceType.HidalgoCounty:
                    case (int)SourceType.ElPasoCounty:
                    case (int)SourceType.FortBendCounty:
                    case (int)SourceType.WilliamsonCounty:
                    case (int)SourceType.GraysonCounty:
                        CommonButtonExecution(siteData, searchItem);
                        break;
                    default:
                        NonDallasButtonExecution(siteData, searchItem, startDate, endingDate);
                        break;
                }

            }
            catch (Exception ex)
            {
                SetStatus(StatusType.Error);
                Console.WriteLine(CommonKeyIndexes.UnexpectedErrorOccurred);
                Console.WriteLine(ex.Message);

                Console.WriteLine(CommonKeyIndexes.DashedLine);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                KillProcess(driverNames);
            }
        }

        private void CommonButtonExecution(WebNavigationParameter siteData, SearchResult searchItem)
        {
            var index = cboSearchType.SelectedIndex;
            var searchType = DallasSearchProcess.GetCourtName(index);
            var keys = new List<WebNavigationKey> {
                new() { Name = "StartDate", Value = searchItem.StartDate},
                new() { Name = "EndDate", Value = searchItem.EndDate },
                new() { Name = "CourtType", Value = searchType }
            };
            var wb = new WebNavigationParameter { Keys = keys };
            IWebInteractive dweb = siteData.Id switch
            {
                130 => new GraysonUiInteractive(wb),
                120 => new WilliamsonUiInteractive(wb),
                110 => new FortBendUiInteractive(wb),
                100 => new ElPasoUiInteractive(wb),
                _ => new HidalgoUiInteractive(wb)
            };
            _ = Task.Run(async () =>
            {
                await ProcessAsync(dweb, siteData, searchItem);
            }).ConfigureAwait(true);
        }

        private void BexarButtonExecution(WebNavigationParameter siteData, SearchResult searchItem)
        {
            var index = cboSearchType.SelectedIndex;
            var searchType = DallasSearchProcess.GetCourtName(index);
            var keys = new List<WebNavigationKey> {
                new() { Name = "StartDate", Value = searchItem.StartDate},
                new() { Name = "EndDate", Value = searchItem.EndDate },
                new() { Name = "CourtType", Value = searchType }
            };
            var wb = new WebNavigationParameter { Keys = keys };
            var dweb = new BexarUiInteractive(wb);
            _ = Task.Run(async () =>
            {
                await ProcessAsync(dweb, siteData, searchItem);
            }).ConfigureAwait(true);
        }

        private void DallasButtonExecution(WebNavigationParameter siteData, SearchResult searchItem)
        {
            var index = cboSearchType.SelectedIndex;
            var searchType = DallasSearchProcess.GetCourtName(index);
            var keys = new List<WebNavigationKey> {
                new() { Name = "StartDate", Value = searchItem.StartDate},
                new() { Name = "EndDate", Value = searchItem.EndDate },
                new() { Name = "CourtType", Value = searchType }
            };
            var wb = new WebNavigationParameter { Keys = keys };
            var dweb = new DallasUiInteractive(wb);
            _ = Task.Run(async () =>
            {
                await ProcessAsync(dweb, siteData, searchItem);
            }).ConfigureAwait(true);
        }

        private void TravisButtonExecution(WebNavigationParameter siteData, SearchResult searchItem, DateTime startDate, DateTime endingDate)
        {
            var dto = (Thompson.RecordSearch.Utility.Dto.DropDown)cboSearchType.SelectedItem;
            var txt = dto.Name.Split(' ')[0];
            var searchType = txt.ToUpper(CultureInfo.CurrentCulture).Trim();
            var search = new TravisSearchProcess();
            search.Search(startDate, endingDate, searchType);
            var dweb = search.GetUiInteractive();
            _ = Task.Run(async () =>
            {
                await ProcessAsync(dweb, siteData, searchItem);
            }).ConfigureAwait(true);
        }

        private void NonDallasButtonExecution(WebNavigationParameter siteData, SearchResult searchItem, DateTime startDate, DateTime endingDate)
        {
            const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
            var isDentonCounty = siteData.Id == (int)SourceType.DentonCounty;
            var keys = siteData.Keys;
            var isDistrictSearch = keys.Find(x =>
                x.Name.Equals(CommonKeyIndexes.DistrictSearchType, // "DistrictSearchType"
                ccic)) != null;
            var criminalToggle = keys.Find(x =>
                x.Name.Equals(CommonKeyIndexes.CriminalCaseInclusion,
                ccic));
            if (isDentonCounty && criminalToggle != null)
            {
                criminalToggle.Value = isDistrictSearch ?
                    CommonKeyIndexes.NumberZero :
                    CommonKeyIndexes.NumberOne;
            }

            if (!isDentonCounty && criminalToggle != null)
            {
                criminalToggle.Value = CommonKeyIndexes.NumberOne;
            }

            IWebInteractive webmgr =
            WebFetchingProvider.
                GetInteractive(siteData, startDate, endingDate);
            _ = Task.Run(async () =>
            {
                await ProcessAsync(webmgr, siteData, searchItem);
            }).ConfigureAwait(true);
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

        private static void ShowNoDataErrorBox()
        {
            MessageBox.Show(CommonKeyIndexes.PleaseCheckSourceDataNotFound,
                CommonKeyIndexes.DataNotFound,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // method is intentionally left blank
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
                case (int)SourceType.HarrisCriminal:
                    using (var hcc = new FormHcc())
                    {
                        hcc.Icon = Icon;
                        hcc.ShowDialog(this);
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

        private async Task ProcessAsync(
            IWebInteractive webmgr,
            WebNavigationParameter siteData,
            SearchResult searchItem)
        {
            try
            {
                CaseData = await Task.Run(() =>
                {
                    return webmgr.Fetch();
                }).ConfigureAwait(true);

                var nonactors = new List<int> {
                    (int)SourceType.DallasCounty,
                    (int)SourceType.TravisCounty,
                    (int)SourceType.BexarCounty,
                    (int)SourceType.HidalgoCounty,
                    (int)SourceType.ElPasoCounty,
                    (int)SourceType.FortBendCounty,
                    (int)SourceType.WilliamsonCounty,
                    (int)SourceType.GraysonCounty,
                };

                ProcessEndingMessage();
                SetStatus(StatusType.Finished);
                KillProcess(CommonKeyIndexes.ChromeDriver);
                if (CaseData == null)
                {
                    throw new KeyNotFoundException(CommonKeyIndexes.NoDataFoundFromCaseExtract);
                }
                if (string.IsNullOrEmpty(CaseData.Result))
                {
                    throw new KeyNotFoundException(CommonKeyIndexes.NoDataFoundFromCaseExtract);
                }
                if (IsEmpty(CaseData))
                {
                    throw new KeyNotFoundException(CommonKeyIndexes.NoDataFoundFromCaseExtract);
                }
                CaseData.WebsiteId = siteData.Id;
                // write search count to api
                var count = CaseData.PeopleList.Count;
                if (count > 0)
                {
                    var member = (SourceType)siteData.Id;
                    var userName = GetUserName();
                    if (string.IsNullOrWhiteSpace(userName)) { userName = "unknown"; }
                    UsageIncrementer.IncrementUsage(userName, member.GetCountyName(), count);
                    UsageReader.WriteUserRecord();
                }
                if (!nonactors.Contains(siteData.Id))
                {
                    ExcelWriter.WriteToExcel(CaseData);
                }
                searchItem.ResultFileName = CaseData.Result;
                searchItem.IsCompleted = true;
                searchItem.MoveToCommon();
                GetObject<List<SearchResult>>(Tag).Add(searchItem);
                ComboBox_DataSourceChanged(null, null);

                var result = MessageBox.Show(
                    CommonKeyIndexes.CaseExtractCompleteWouldYouLikeToView,
                    CommonKeyIndexes.DataExtractSuccess,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
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
                Console.WriteLine(CommonKeyIndexes.UnexpectedErrorOccurred);
                Console.WriteLine(ex.Message);

                Console.WriteLine(CommonKeyIndexes.DashedLine);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {

                KillProcess(CommonKeyIndexes.ChromeDriver);
            }
        }

        private void SetStatusFromOffThread(StatusState v)
        {
            this.Invoke(new Action(() =>
            {
                toolStripStatus.Text = string.Format(CultureInfo.CurrentCulture, "{0}", v.Name);
                toolStripStatus.ForeColor = v.Color;
                Refresh();
            }));
        }
        private static readonly SessionUsagePersistence UsageIncrementer
            = SessionPersistenceContainer.GetContainer
            .GetInstance<SessionUsagePersistence>();
        private static readonly SessionMonthToDatePersistence UsageReader
            = SessionPersistenceContainer.GetContainer
            .GetInstance<SessionMonthToDatePersistence>();
    }
}
