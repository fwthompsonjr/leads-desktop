using System;
using System.Collections.Generic;
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

        private static System.Timers.Timer aTimer;

        private void SetUpTimer()
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
                cboWebsite.SelectedIndex = Convert.ToInt32(configIndex);
            } 
#endif
            SetUpTimer();
        }

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Application.DoEvents();
        }

        private void CboWebsite_SelectedValueChanged(object sender, EventArgs e)
        {
            var source = (WebNavigationParameter)cboWebsite.SelectedItem;
            var customList = new List<int>
            {
                (int)SourceType.CollinCounty,
                (int)SourceType.TarrantCounty
            };
            ButtonDentonSetting.Visible = (source.Id == (int)SourceType.DentonCounty | source.Id == (int)SourceType.CollinCounty);
            cboSearchType.Visible = source.Id == (int)SourceType.CollinCounty;
            cboCaseType.Visible = customList.Contains(source.Id);
            labelCboCaseType.Text = source.Id == (int)SourceType.TarrantCounty ? "Custom Search" : "Search Type";

            cboCourts.Visible = source.Id == (int)SourceType.TarrantCounty;
            // cboCourts
            var showList = new List<int> 
            {
                4,
                5
            };
            for (int i = 3; i <= 5; i++)
            {
                tableLayoutPanel1.RowStyles[i].SizeType = SizeType.Absolute;
                tableLayoutPanel1.RowStyles[i].Height = source.Id == (int)SourceType.CollinCounty ? 49 : 0;
                if (showList.Contains(i))
                {
                    tableLayoutPanel1.RowStyles[i].Height = source.Id == (int)SourceType.TarrantCounty ? 49 : 0;
                }
            }
            tsStatusLabel.Text = string.Empty;
            // when in Denton County write Settings
            if (source.Id == (int)SourceType.DentonCounty)
            {
                ButtonDentonSetting.Text = "Settings";
                SetDentonStatusLabelFromSetting();
            } else
            {
                ButtonDentonSetting.Text = "Password";
            }

            if (!customList.Contains(source.Id)) return;
            // custom combo mapping for case type
            var caseTypeName = source.Id == (int)SourceType.CollinCounty ?
                "collinCountyCaseType" :
                "tarrantCountyCustomType";
            var caseTypes = CaseTypeSelectionDto.GetDto(caseTypeName);
            cboCaseType.DataSource = caseTypes.DropDowns.First().Options;
            cboCaseType.DisplayMember = "Name";
            cboCaseType.ValueMember = "Id";
        }

        internal void SetDentonStatusLabelFromSetting()
        {
            var sb = new StringBuilder();
            var searchDto = SearchSettingDto.GetDto();
            var showDistrict = searchDto.CountySearchTypeId == 1;
            var courtNames = CaseTypeSelectionDto.GetDto(
                showDistrict ? "dentonDistrictCaseType" : "dentonCountyCaseType");
            sb.AppendFormat(
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

            sb.AppendFormat(" - {0}", nameDropDown.Name);
            if (showDistrict)
            {
                sb.AppendFormat(" - {0}", "Criminal,Civil and Family".Split(',')[subItemId]);
            }
            tsStatusLabel.Text = sb.ToString();
        }

        private void CboSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var source = (DropDown)cboSearchType.SelectedItem;
            cboCaseType.DataSource = source.Options;
            cboCaseType.DisplayMember = "Name";
            cboCaseType.ValueMember = "Id";
            cboCaseType.SelectedIndex = 0;
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
                " Start time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")
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
                " End time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")
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
