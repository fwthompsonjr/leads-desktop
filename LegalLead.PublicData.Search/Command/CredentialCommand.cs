using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Command
{
    internal class CredentialCommand : CommandBase
    {
        public override string Name => "Credentials";

        public override void Execute(FormMain mainForm)
        {
            {
                using (var form = new FormCredential())
                {
                    Application.Run(form);
                }
            }
        }
    }
}
