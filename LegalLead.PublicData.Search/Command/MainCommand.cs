using System.Windows.Forms;
using Thompson.RecordSearch.Utility;

namespace LegalLead.PublicData.Search.Command
{
    public class MainCommand : CommandBase
    {
        public override string Name => CommonKeyIndexes.FormNameMain; // "Main";

        public override void Execute(FormMain mainForm)
        {
            Application.Run(mainForm);
        }
    }
}
