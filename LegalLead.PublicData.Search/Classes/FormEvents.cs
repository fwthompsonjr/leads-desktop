using LegalLead.PublicData.Search.Classes;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FormMain : Form
    {

        internal void SetDentonStatusLabelFromSetting()
        {
            var srcId = ((WebNavigationParameter)cboWebsite.SelectedItem).Id;
            if(srcId != (int)SourceType.DentonCounty)
            {
                return;
            }
            var sb = new StringBuilder();
            var searchDto = SearchSettingDto.GetDto();
            var showDistrict = searchDto.CountySearchTypeId == 1;
            var courtNames = CaseTypeSelectionDto.GetDto(
                showDistrict ? "dentonDistrictCaseType" : "dentonCountyCaseType");
            sb.AppendFormat(
                System.Globalization.CultureInfo.CurrentCulture,
                "Search: {0}.", showDistrict ?
                "District Courts" : "JP - County Courts");

            var subItemId = showDistrict ?
                searchDto.DistrictSearchTypeId :
                searchDto.CountySearchTypeId;

            var courtItemId = showDistrict ? 
                searchDto.DistrictCourtId + 1: 
                searchDto.CountyCourtId + 1;

            var courtDropDown = courtNames.DropDowns.First();
            var nameDropDown = courtDropDown.Options.Find(x => x.Id == courtItemId);

            _ = sb.AppendFormat(
                System.Globalization.CultureInfo.CurrentCulture,
                " - {0}", nameDropDown.Name);
            if (showDistrict)
            {
                _ = sb.AppendFormat(
                System.Globalization.CultureInfo.CurrentCulture,
                " - {0}", "Criminal,Civil and Family".Split(',')[subItemId]);
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
            cboCaseType.DataSource = source.Options;
            cboCaseType.DisplayMember = "Name";
            cboCaseType.ValueMember = "Id";
            cboCaseType.SelectedIndex = 0;
        }

        private static System.Timers.Timer aTimer;

        private static void SetUpTimer()
        {

            // Create a timer and set a two second interval.
            aTimer = new System.Timers.Timer
            {
                Interval = 2000
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

            var websites = new SettingsManager().GetNavigation();
            var caseTypes = CaseTypeSelectionDto.GetDto("collinCountyCaseType");
            var tarrantCourt = CaseTypeSelectionDto.GetDto("tarrantCountyCaseType");

            cboWebsite.DataSource = websites;
            cboWebsite.DisplayMember = "Name";
            cboWebsite.ValueMember = "Id";

            cboSearchType.Visible = false;
            cboSearchType.DataSource = caseTypes.DropDowns;
            cboSearchType.DisplayMember = "Name";
            cboSearchType.ValueMember = "Id";



            cboCourts.Visible = false;
            cboCourts.DataSource = tarrantCourt.DropDowns.First().Options;
            cboCourts.DisplayMember = "Name";
            cboCourts.ValueMember = "Id";

            cboCaseType.Visible = false;
            cboCaseType.DataSource = caseTypes.DropDowns.First().Options;
            cboCaseType.DisplayMember = "Name";
            cboCaseType.ValueMember = "Id";

            for (int i = 3; i <= 5; i++)
            {
                tableLayoutPanel1.RowStyles[i].SizeType = SizeType.Absolute;
                tableLayoutPanel1.RowStyles[i].Height = 0;
            }

            cboSearchType.SelectedIndexChanged += CboSearchType_SelectedIndexChanged;
            cboWebsite.SelectedValueChanged += CboWebsite_SelectedValueChanged;


            cboSearchType.SelectedIndex = 0;
            cboCourts.SelectedIndex = 0;
#if DEBUG 

            // change selected index based upon appSetting
            var configIndex = ConfigurationManager.AppSettings["form-context-id"];
            if (!string.IsNullOrEmpty(configIndex))
            {
                cboWebsite.SelectedIndex = Convert.ToInt32(
                    configIndex,
                    System.Globalization.CultureInfo.CurrentCulture);
            } 
#endif
            SetUpTimer();
        }

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Application.DoEvents();
        }


        private void ProcessStartingMessage()
        {
            var message = "Starting fetch request: "
                + Environment.NewLine +
                " Website: " + cboWebsite.SelectedText
                + Environment.NewLine +
                " Start Date: " + dteStart.Value.Date.ToShortDateString()
                + Environment.NewLine +
                " End Date: " + dteEnding.Value.Date.ToShortDateString()
                + Environment.NewLine +
                " Start time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss", 
                    System.Globalization.CultureInfo.CurrentCulture)
                + Environment.NewLine +
                "- - - - - - - - - - - - - - - - - - - - - - - - - ";

            Console.WriteLine(message);

        }

        private void ProcessEndingMessage()
        {
            var message = "Ending fetch request: "
                + Environment.NewLine +
                " Website: " + cboWebsite.SelectedText
                + Environment.NewLine +
                " Start Date: " + dteStart.Value.Date.ToShortDateString()
                + Environment.NewLine +
                " End Date: " + dteEnding.Value.Date.ToShortDateString()
                + Environment.NewLine +
                " End time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss", 
                    System.Globalization.CultureInfo.CurrentCulture)
                + Environment.NewLine +
                "- - - - - - - - - - - - - - - - - - - - - - - - - "
                + Environment.NewLine +
                "- - - - - - - - - - - - - - - - - - - - - - - - - ";

            Console.WriteLine(message);

        }

        private void TryOpenExcel()
        {
            var xmlFile = CaseData == null ? string.Empty : CaseData.Result;
            if (string.IsNullOrEmpty(xmlFile))
            {
                MessageBox.Show("File not found error.", "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
                return;
            }
            if (!File.Exists(xmlFile))
            {
                MessageBox.Show("Data source file not found error.", "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
            xmlFile = xmlFile.Replace(".xml", ".xlsx");
            if (!File.Exists(xmlFile))
            {
                MessageBox.Show("Excel source file not found error.", "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            System.Diagnostics.Process.Start(xmlFile);
        }
    }
}
