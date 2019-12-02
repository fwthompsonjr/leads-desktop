using System;
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
            SetStatus(StatusTypes.Ready);
        }

        private void SetStatus(StatusTypes status)
        {
            var v = StatusHelper.GetStatus(status);
            toolStripStatus.Text = string.Format("{0}", v.Name);
            toolStripStatus.ForeColor = v.Color;
            Refresh();
            Application.DoEvents();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            
            try
            {
                SetStatus(StatusTypes.Running);
                if (!ValidateCustom())
                {
                    SetStatus(StatusTypes.Ready);
                    return;
                }
                txConsole.Text = string.Empty;
                ProcessStartingMessage();
                var startDate = dteStart.Value.Date;
                var endingDate = dteEnding.Value.Date;
                var siteData = (WebNavigationParameter)(cboWebsite.SelectedItem);
                IWebInteractive webmgr =
                    WebFetchingProvider.
                    GetInteractive(siteData, startDate, endingDate);

                CaseData = webmgr.Fetch();

                ProcessEndingMessage();
                SetStatus(StatusTypes.Finished);
                if (CaseData == null)
                {
                    throw new ApplicationException("No data found from case extract.");
                }
                if (string.IsNullOrEmpty(CaseData.Result))
                {
                    throw new ApplicationException("No data found from case extract.");
                }

                var writer = new ExcelWriter();
                writer.WriteToExcel(CaseData);

                var result = MessageBox.Show(
                    "Your data extract has completed. Would you like to view in Excel?",
                    "Data extract success",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result != System.Windows.Forms.DialogResult.Yes)
                {
                    SetStatus(StatusTypes.Ready);
                    return;
                }
                TryOpenExcel();
                SetStatus(StatusTypes.Ready);
            }
            catch (Exception ex)
            {
                SetStatus(StatusTypes.Error);
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
                var result = new FormDentonSetting().ShowDialog(this);
                SetDentonStatusLabelFromSetting();
            }
            else
            {
                var result = new FormCredential().ShowDialog(this);
            }
        }
    }
}
