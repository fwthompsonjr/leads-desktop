using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Command
{
    public class MainCommand : CommandBase
    {
        public override string Name => "Main";

        public override void Execute(FormMain mainForm)
        {
            Application.Run(mainForm);
        }
    }
}
