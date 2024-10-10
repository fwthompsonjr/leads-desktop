using LegalLead.PublicData.Search.Classes;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;

namespace LegalLead.PublicData.Search.Command
{
    public class MainCommand : CommandBase
    {
        public override string Name => CommonKeyIndexes.FormNameMain;

        public override void Execute(FormMain mainForm)
        {
            ShortcutGenerator.Generate();
            Application.Run(mainForm);
        }
    }
}
