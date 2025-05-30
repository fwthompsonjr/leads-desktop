using LegalLead.PublicData.Search.Classes;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FormMain : Form
    {

        private void TryHideProgress()
        {
            try
            {
                HideProgress();
            }
            catch
            {
                Invoke(HideProgress);
            }
        }

        private void ClearProgressDate()
        {
            try
            {
                lbProgressDate.Text = string.Empty;
            }
            catch
            {
                Invoke(() => { lbProgressDate.Text = string.Empty; });
            }
        }

        private void TryShowProgress(int min, int max, int current, string dateIndication)
        {
            try
            {
                if (max == 0 || current > max) return;
                ShowProgress(min, max, current, dateIndication);
            }
            catch
            {
                Invoke(() => { ShowProgress(min, max, current, dateIndication); });
            }
        }

        private void HideProgress()
        {
            if (!string.IsNullOrEmpty(lbProgressDate.Text)) return;
            progressBar1.Visible = false;
            labelProgress.Visible = false;
            tableLayoutPanel1.RowStyles[RowsIndexes.ProgressRowId].Height = 0;
        }

        private void ShowProgress(int min, int max, int current, string dateIndication = "")
        {
            labelProgress.Visible = true;
            lbProgressDate.Visible =
                !string.IsNullOrEmpty(lbProgressDate.Text) ||
                !string.IsNullOrEmpty(dateIndication);
            if (dateIndication == "hide")
            {
                lbProgressDate.Text = string.Empty;
                lbProgressDate.Visible = false;
                TryHideProgress();
                return;
            }
            if (!string.IsNullOrEmpty(dateIndication)) lbProgressDate.Text = dateIndication;
            ControlExtensions.Suspend(progressBar1);
            tableLayoutPanel1.RowStyles[RowsIndexes.ProgressRowId].Height = 40;
            progressBar1.Visible = false;
            progressBar1.Minimum = min;
            progressBar1.Maximum = max;
            progressBar1.Value = current;
            progressBar1.Visible = true;
            ControlExtensions.Resume();
        }


    }
}
