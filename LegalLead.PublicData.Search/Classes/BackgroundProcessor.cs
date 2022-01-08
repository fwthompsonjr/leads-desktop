using System;
using System.ComponentModel;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FormMain : Form
    {

        class BackgroundProcessor
        {
            public BackgroundProcessor(IWebInteractive webManager, FormMain parentForm)
            {
                WebManager = webManager;
                ParentForm = parentForm;
            }

            public IWebInteractive WebManager { get; }
            public FormMain ParentForm { get; }

            public void Execute()
            {

                using (var bworker = new BackgroundWorker())
                {
                    bworker.DoWork += ParentForm.BackgroundFetchData;
                    bworker.RunWorkerCompleted += ParentForm.BackgroundFetchCompleted;
                    bworker.RunWorkerAsync(WebManager);
                }
            }


        }


        protected void BackgroundFetchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            try
            {
                if (e == null)
                {
                    return;
                }

                if (e.Error != null)
                {
                    throw e.Error;
                }

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
                CaseData.WebsiteId = ((WebNavigationParameter)cboWebsite.SelectedItem).Id;
                ExcelWriter.WriteToExcel(CaseData);

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
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                SetStatus(StatusType.Error);
                Console.WriteLine(CommonKeyIndexes.UnexpectedErrorOccurred);
                Console.WriteLine(ex.Message);

                Console.WriteLine(CommonKeyIndexes.DashedLine); // "- - - - - - - - - - - - - - - - -");
                Console.WriteLine(ex.StackTrace);
            }
#pragma warning restore CA1031 // Do not catch general exception types
            finally
            {

                KillProcess(CommonKeyIndexes.ChromeDriver);
            }
        }

        private void BackgroundFetchData(object sender, DoWorkEventArgs e)
        {
            var webmgr = (IWebInteractive)e.Argument;
            CaseData = webmgr.Fetch();
        }



        delegate void SetTextCallback(string text);

        public void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (txConsole.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Invoke(d, new object[] { text });
            }
            else
            {
                txConsole.Text = text;
            }
        }
    }
}
