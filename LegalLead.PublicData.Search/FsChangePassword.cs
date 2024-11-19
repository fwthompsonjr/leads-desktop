using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FsChangePassword : Form
    {
        public FsChangePassword()
        {
            InitializeComponent();
            _statusText = lbStatus.Text;
            _model = new()
            {
                UserName = GetUserName()
            };

        }

        private readonly UserPasswordChangeModel _model;
        private readonly string _statusText;

        private void PopulateModel()
        {
            var list = InputControls();
            list.ForEach(x =>
            {
                var indx = list.IndexOf(x) + 1;
                var txt = x.Text ?? string.Empty;
                _model[indx] = txt;
            });
        }

        private List<MaskedTextBox> InputControls()
        {
            var collection = new[]
            {
                tbxCurrentPwd,
                tbxPwd,
                tbxConfirmPwd,
            }.ToList();
            return collection;
        }

        private static string GetUserName()
        {
            var container = AuthenicationContainer.GetContainer;
            var userservice = container.GetInstance<SessionUserPersistence>();
            return userservice.GetUserName();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            PopulateModel();
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            lbStatus.Text = _statusText;
            btnSubmit.Enabled = false;
            PopulateModel();
            var errors = _model.Validate(out var isvalid);
            if (!isvalid)
            {
                var message = string.Join(Environment.NewLine, errors.Select(x => x.ErrorMessage));
                lbStatus.Text = message;
                return;
            }
            btnSubmit.Enabled = true;
        }
    }
}
