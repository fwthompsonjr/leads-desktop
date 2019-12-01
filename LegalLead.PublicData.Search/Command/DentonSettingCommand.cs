using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Command
{
    public class DentonSettingCommand : CommandBase
    {
        public override string Name => "Denton";

        public override void Execute(FormMain mainForm)
        {
            var form = new FormDentonSetting();
            Application.Run(form);
        }
    }
}
