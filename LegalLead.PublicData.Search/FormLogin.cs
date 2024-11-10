using LegalLead.PublicData.Search.Helpers;
using System;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            ToggleEnabled(false);
            try
            {
                var service = AuthenicationContainer.GetContainer.GetInstance<IAuthenicationService>();
                if (service is AuthenicationService authenicationService && authenicationService.RetryCount <= 0)
                {
                    Environment.Exit(0);
                }
                var uid = tbxUser.Text;
                var pword = tbxPwd.Text;
                var response = service.LoginAsync(uid, pword).Result;
                if (response) return;
            }
            finally
            {
                ToggleEnabled(true);
            }
        }

        private void ToggleEnabled(bool enabled)
        {
            btnSubmit.Enabled = enabled;
            tbxUser.Enabled = enabled;
            tbxPwd.Enabled = enabled;
        }
    }
}
