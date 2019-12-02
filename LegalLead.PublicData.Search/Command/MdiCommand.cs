using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Command
{
    public class MdiCommand : CommandBase
    {
        public override string Name => "mdi";

        public override void Execute(FormMain mainForm)
        {
            var mdi = new MDIParent();
            mainForm.MdiParent = mdi;
            mainForm.WindowState = FormWindowState.Maximized;
            mainForm.Text = "Settings";
            mainForm.Show();
            Application.Run(mdi);

        }
    }
}
