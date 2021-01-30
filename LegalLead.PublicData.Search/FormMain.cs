using LegalLead.PublicData.Search.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
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
            Text = string.Format(CultureInfo.CurrentCulture, @"{0} - {1}",
                appName.Name, version);

            BindComboBoxes();
            SetDentonStatusLabelFromSetting();
            SetStatus(StatusType.Ready);
        }

        protected static T GetObject<T>(object item)
        {
            return (T)item;
        }
        private void SetStatus(StatusType status)
        {
            var v = StatusHelper.GetStatus(status);
            toolStripStatus.Text = string.Format(CultureInfo.CurrentCulture, "{0}", v.Name);
            toolStripStatus.ForeColor = v.Color;
            Refresh();
            Application.DoEvents();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private void Button1_Click(object sender, EventArgs e)
        {
            
            try
            {

                KillProcess(CommonKeyIndexes.ChromeDriver);
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
                const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
                var isDentonCounty = siteData.Id == (int)SourceType.DentonCounty;
                var keys = siteData.Keys;
                var isDistrictSearch = keys.FirstOrDefault(x => 
                    x.Name.Equals(CommonKeyIndexes.DistrictSearchType, // "DistrictSearchType"
                    ccic)) != null;
                var criminalToggle = keys.FirstOrDefault(x => 
                    x.Name.Equals(CommonKeyIndexes.CriminalCaseInclusion, 
                    ccic));
                if (isDentonCounty && criminalToggle != null)
                {
                    criminalToggle.Value = isDistrictSearch ?
                        CommonKeyIndexes.NumberZero :
                        CommonKeyIndexes.NumberOne; // "0" : "1";
                }

                if(!isDentonCounty & criminalToggle != null)
                {
                    criminalToggle.Value = CommonKeyIndexes.NumberOne;
                }

                IWebInteractive webmgr =
                    WebFetchingProvider.
                    GetInteractive(siteData, startDate, endingDate);

                CaseData = webmgr.Fetch();

                ProcessEndingMessage();
                SetStatus(StatusType.Finished);
                KillProcess(CommonKeyIndexes.ChromeDriver);
                if (CaseData == null)
                {
                    throw new ApplicationException(CommonKeyIndexes.NoDataFoundFromCaseExtract);
                }
                if (string.IsNullOrEmpty(CaseData.Result))
                {
                    throw new ApplicationException(CommonKeyIndexes.NoDataFoundFromCaseExtract);
                }
                CaseData.WebsiteId = siteData.Id;
                ExcelWriter.WriteToExcel(CaseData);
                searchItem.ResultFileName = CaseData.Result;
                searchItem.IsCompleted = true;
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

                Console.WriteLine(CommonKeyIndexes.DashedLine); // "- - - - - - - - - - - - - - - - -");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {

                KillProcess(CommonKeyIndexes.ChromeDriver);
            }
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
                return;
            }
        }

        private void ShowNoDataErrorBox()
        {
            MessageBox.Show(CommonKeyIndexes.PleaseCheckSourceDataNotFound,
                CommonKeyIndexes.DataNotFound,
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
                using (var credential = new FormDentonSetting())
                {
                    credential.Icon = Icon;
                    credential.ShowDialog(this);
                    SetDentonStatusLabelFromSetting();
                }
            }
            else
            {
                using (var result = new FormCredential())
                {
                    result.Icon = Icon;
                    result.ShowDialog(this); 
                }
            }
        }

        private void KillProcess(string processName)
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
    }
}
