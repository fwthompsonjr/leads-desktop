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
            mainForm.MdiParent = mdi;
            mainForm.WindowState = FormWindowState.Maximized;
            mainForm.Text = CommonKeyIndexes.SettingsLabel; // "Settings";
            mainForm.Show();
            Application.Run(mdi);

        }
    }
}
