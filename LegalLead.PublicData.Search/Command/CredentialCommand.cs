using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Command
{
    class CredentialCommand : CommandBase
    {
        public override string Name => "Credentials";

        public override void Execute(FormMain mainForm)
        {
            Application.Run(new FormCredential());
        }
    }
}
