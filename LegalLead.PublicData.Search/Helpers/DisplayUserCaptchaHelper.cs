using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Helpers
{
    using Rx = Properties.Resources;
    internal static class DisplayUserCaptchaHelper
    {
        [ExcludeFromCodeCoverage]
        public static bool UserPrompt()
        {
            var response = DialogResult.None;
            while (response != DialogResult.OK)
            {
                response = MessageBox.Show(
                    Rx.UI_CAPTCHA_DESCRIPTION,
                    Rx.UI_CAPTCHA_TITLE,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.ServiceNotification);
                if (response == DialogResult.Cancel) break;
            }
            return response == DialogResult.OK;
        }
    }
}
