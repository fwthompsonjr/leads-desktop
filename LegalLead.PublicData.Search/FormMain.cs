﻿using LegalLead.PublicData.Search.Classes;
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
            BindComboBoxes();
            SetDentonStatusLabelFromSetting();
            SetStatus(StatusType.Ready);
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
                    case (int)SourceType.DallasCounty:
                        DallasButtonExecution(siteData, searchItem);
                        break;
                    case (int)SourceType.TravisCounty:
                        TravisButtonExecution(siteData, searchItem, startDate, endingDate);
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

        private void DallasButtonExecution(WebNavigationParameter siteData, SearchResult searchItem)
        {
            var index = cboSearchType.SelectedIndex;
            var searchType = DallasSearchProcess.GetCourtName(index);
            var keys = new List<WebNavigationKey> {
                new WebNavigationKey() { Name = "StartDate", Value = searchItem.StartDate},
                new WebNavigationKey() { Name = "EndDate", Value = searchItem.EndDate },
                new WebNavigationKey() { Name = "CourtType", Value = searchType }
            };
            var wb = new WebNavigationParameter { Keys = keys };
            var dweb = new DallasUiInteractive(wb);
            Task.Run(async () =>
            {
                await ProcessAsync(dweb, siteData, searchItem);
            }).ConfigureAwait(true);
        }

        private void TravisButtonExecution(WebNavigationParameter siteData, SearchResult searchItem, DateTime startDate, DateTime endingDate)
        {
            var searchType = cboSearchType.SelectedText.ToUpper(CultureInfo.CurrentCulture).Trim();
            var search = new TravisSearchProcess();
            search.Search(startDate, endingDate, searchType);
            var dweb = search.GetUiInteractive();
            Task.Run(async () =>
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
            Task.Run(async () =>
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
                    (int)SourceType.TravisCounty
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

    }
}
