using System.Windows.Forms;
using Thompson.RecordSearch.Utility;

namespace LegalLead.PublicData.Search.Command
{
    public class DentonSettingCommand : CommandBase
    {
        public override string Name => CommonKeyIndexes.FormNameDenton; // "Denton";

        public override void Execute(FormMain mainForm)
        {
            using (var form = new FormDentonSetting())
            {
                Application.Run(form);
            }
        }
    }
}
