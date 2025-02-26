using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FormMain : Form
    {

        //class BackgroundProcessor
        //{
        //    public BackgroundProcessor(IWebInteractive webManager, FormMain parentForm)
        //    {
        //        WebManager = webManager;
        //        ParentForm = parentForm;
        //    }

        //    public IWebInteractive WebManager { get; }
        //    public FormMain ParentForm { get; }

        //    public void Execute()
        //    {

        //        using (var bworker = new BackgroundWorker())
        //        {
        //            bworker.DoWork += ParentForm.BackgroundFetchData;
        //            bworker.RunWorkerCompleted += ParentForm.BackgroundFetchCompleted;
        //            bworker.RunWorkerAsync(WebManager);
        //        }
        //    }


        //}



        delegate void SetTextCallback(string text);

        public void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (txConsole.InvokeRequired)
            {
                SetTextCallback d = new(SetText);
                Invoke(d, new object[] { text });
            }
            else
            {
                txConsole.Text = text;
            }
        }
    }
}
