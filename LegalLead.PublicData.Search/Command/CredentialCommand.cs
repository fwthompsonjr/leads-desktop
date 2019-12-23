using System.Windows.Forms;
using Thompson.RecordSearch.Utility;

namespace LegalLead.PublicData.Search.Command
{
    internal class CredentialCommand : CommandBase
    {
        public override string Name => CommonKeyIndexes.FormNameCredentials; // "Credentials";

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
