using LegalLead.PublicData.Search.Classes;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FormMain : Form
    {

        internal void SetDentonStatusLabelFromSetting()
        {
            const int One = 1;
            var srcId = ((WebNavigationParameter)cboWebsite.SelectedItem).Id;
            if(srcId != (int)SourceType.DentonCounty)
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

            var courtDropDown = courtNames.DropDowns.First();
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
            if (changeHandler == null) return;
            changeHandler.Change();
        }

        private void CboSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var source = (DropDown)cboSearchType.SelectedItem;
            
            var selectedItem = (WebNavigationParameter)cboWebsite.SelectedItem;
            if (selectedItem != null && selectedItem.Id == (int)SourceType.HarrisCivil)
            {
                // rebind ??
                return;
            }

            var caseTypes = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.CollinCountyCaseType);

            cboSearchType.DataSource = caseTypes.DropDowns;
            cboSearchType.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboSearchType.ValueMember = CommonKeyIndexes.IdProperCase;
            cboSearchType.SelectedItem = selectedItem;

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
            var caseTypes = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.CollinCountyCaseType);
            var tarrantCourt = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.TarrantCountyCaseType);
            const int Zero = 0;
            cboWebsite.DataSource = websites;
            cboWebsite.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboWebsite.ValueMember = CommonKeyIndexes.IdProperCase;

            cboSearchType.Visible = false;
            cboSearchType.DataSource = caseTypes.DropDowns;
            cboSearchType.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboSearchType.ValueMember = CommonKeyIndexes.IdProperCase;



            cboCourts.Visible = false;
            cboCourts.DataSource = tarrantCourt.DropDowns.First().Options;
            cboCourts.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboCourts.ValueMember = CommonKeyIndexes.IdProperCase;

            cboCaseType.Visible = false;
            cboCaseType.DataSource = caseTypes.DropDowns.First().Options;
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
#if DEBUG
            DebugFormLoad();
#endif
            SetUpTimer();
        }

        private void DebugFormLoad()
        {

            // change selected index based upon appSetting
            var configIndex = ConfigurationManager.AppSettings[CommonKeyIndexes.FormContextId];
            var startDate = ConfigurationManager.AppSettings[CommonKeyIndexes.FormStartDate];
            var endDate = ConfigurationManager.AppSettings[CommonKeyIndexes.FormEndDate];
            if (!string.IsNullOrEmpty(configIndex))
            {
                cboWebsite.SelectedIndex = Convert.ToInt32(
                    configIndex,
                    CultureInfo.CurrentCulture);
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

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Application.DoEvents();
        }


        protected void ProcessStartingMessage()
        {
            var source = (WebNavigationParameter)cboWebsite.SelectedItem;
            var message = CommonKeyIndexes.StartingFetchRequest
                + Environment.NewLine + " " +
                CommonKeyIndexes.WebsiteLabel + source.Name
                + Environment.NewLine + " " +
                CommonKeyIndexes.StartDateLabel + dteStart.Value.Date.ToShortDateString()
                + Environment.NewLine + " " +
                CommonKeyIndexes.EndDateLabel + dteEnding.Value.Date.ToShortDateString()
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
            var source = (WebNavigationParameter)cboWebsite.SelectedItem;
            var message = CommonKeyIndexes.EndingFetchRequest
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

            Console.WriteLine(message);

        }

        private void TryOpenExcel()
        {
            var xmlFile = CaseData == null ? string.Empty : CaseData.Result;
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
            if (!File.Exists(xmlFile))
            {
                MessageBox.Show(
                    CommonKeyIndexes.ExcelSourceNotFoundError, // "Excel source file not found error.", 
                    CommonKeyIndexes.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            System.Diagnostics.Process.Start(xmlFile);
        }
    }
}
