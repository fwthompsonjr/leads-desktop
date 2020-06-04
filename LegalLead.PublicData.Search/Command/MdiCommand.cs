using System.Windows.Forms;
using Thompson.RecordSearch.Utility;

namespace LegalLead.PublicData.Search.Command
{
    public class MdiCommand : CommandBase
    {
        public override string Name => CommonKeyIndexes.FormNameMdi;

        public override void Execute(FormMain mainForm)
        {
            if (mainForm == null) throw new System.ArgumentNullException(nameof(mainForm));
            var mdi = new MDIParent();
            // hide the status bar
            mdi.toolBarToolStripMenuItem.Checked = false;
            mdi.statusBarToolStripMenuItem.Checked = false;
            mdi.statusStrip.Visible = false;
            mdi.toolStrip.Visible = false;
            mainForm.MdiParent = mdi;
            mainForm.WindowState = FormWindowState.Maximized;
            mainForm.Text = CommonKeyIndexes.SettingsLabel; // "Settings";
            mainForm.FormClosed += MainForm_FormClosed;
            mainForm.Show();
            Application.Run(mdi);

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms == null) return;
            if (Application.OpenForms.Count == 1)
            {
                Application.Exit();
            }
        }
    }
}
