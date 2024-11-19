using LegalLead.PublicData.Search.Helpers;
using Newtonsoft.Json;
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
        internal bool DebugMode { get; set; }
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            ToggleEnabled(false);
            SetStatus(0);
            try
            {
                var container = AuthenicationContainer.GetContainer;
                var service = container.GetInstance<IAuthenicationService>();
                var userservice = container.GetInstance<SessionUserPersistence>();
                userservice.Initialize();
                if (service.RetryCount <= 0)
                {
                    DialogResult = DialogResult.Cancel;
                    return;
                }
                var uid = tbxUser.Text;
                var pword = tbxPwd.Text;
                if (DebugMode && string.IsNullOrEmpty(uid) && string.IsNullOrEmpty(pword))
                {
                    string decodec = GetDebugAccount();
                    SessionUtil.Write(decodec);
                    DialogResult = DialogResult.OK;
                    return;
                }
                if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(pword))
                {
                    SetStatus(1);
                    return;
                }
                var response = service.Login(uid, pword);
                SetStatus(response ? 2 : 1);
                if (response)
                {
                    var dto = new LoginAccountDto { UserName = uid };
                    userservice.Write(JsonConvert.SerializeObject(dto));
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            finally
            {
                ToggleEnabled(true);
            }
        }

        private static string GetDebugAccount()
        {
            var mode = ApiHelper.ApiMode;
            if (mode == "legacy")
            {
                var txt = Properties.Resources.debug_account;
                var decodec = txt.Decrypt();
                return decodec;
            }
            return DebugAssistant.GetBo();
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
