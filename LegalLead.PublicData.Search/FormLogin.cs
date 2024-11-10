using LegalLead.PublicData.Search.Helpers;
using System;
using System.Collections.Generic;
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
            SetStatus(0);
            try
            {
                var service = AuthenicationContainer.GetContainer.GetInstance<IAuthenicationService>();
                if (service is AuthenicationService authenicationService && authenicationService.RetryCount <= 0)
                {
                    DialogResult = DialogResult.Cancel;
                    return;
                }
                var uid = tbxUser.Text;
                var pword = tbxPwd.Text;
                if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(pword))
                {
                    SetStatus(1);
                    return;
                }
                var response = service.Login(uid, pword);
                SetStatus(response ? 2 : 1);
                if (response)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
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

        private void SetStatus(int statusId)
        {
            if (statusId < 0 || statusId > statusItems.Count - 1) return;
            labelSts.Text = statusItems[statusId];
        }

        private static readonly List<string> statusItems = new List<string>
        {
            "Please enter credentials",
            "Invalid attempt. Please check values",
            "Login succeeded"
        };
    }
}
