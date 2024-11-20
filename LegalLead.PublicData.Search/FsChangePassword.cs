using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
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
            txUserName.Text = _model.UserName;
            Shown += FsChangePassword_Shown;
            _statusColor = lbStatus.ForeColor;
            _textColor = tbxConfirmPwd.ForeColor;
        }

        private void FsChangePassword_Shown(object sender, EventArgs e)
        {
            InputControls()[0].Focus();
        }

        private readonly UserPasswordChangeModel _model;
        private readonly string _statusText;
        private readonly Color _statusColor;
        private readonly Color _textColor;
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
            if (maskedBoxes != null) return maskedBoxes;
            maskedBoxes = new[]
            {
                tbxCurrentPwd,
                tbxPwd,
                tbxConfirmPwd,
            }.ToList();
            return maskedBoxes;
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
            if (sender is MaskedTextBox tbx && tbx.ForeColor != _textColor)
            {
                ValidateEntries(out var _);
                tbx.ForeColor = _textColor;
            }
            if (!btnSubmit.Enabled) { btnSubmit.Enabled = true; }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            lbStatus.Text = _statusText;
            lbStatus.ForeColor = _statusColor;
            btnSubmit.Enabled = false;
            PopulateModel();
            var isvalid = ValidateEntries(out var errors);
            if (isvalid)
            {
                var rsp = changeService.ChangePassword(_model);
                lbStatus.Text = rsp.Message;
                btnSubmit.Enabled = true;
                return;
            }
            lbStatus.ForeColor = Color.Red;
            var controls = InputControls();
            var issues = new[]
            {
                errors.Exists(a => a.MemberNames.Contains("OldPassword")),
                errors.Exists(a => a.MemberNames.Contains("NewPassword")),
                errors.Exists(a => a.MemberNames.Contains("ConfirmPassword")),
            };
            for (var i = 0; i < issues.Length; i++)
            {
                var tbx = controls[i];
                var isError = issues[i];
                if (isError) { tbx.ForeColor = Color.Red; }
                else { tbx.ForeColor = _textColor; }
            }
        }

        private bool ValidateEntries(out List<ValidationResult> errors)
        {
            PopulateModel();
            errors = _model.Validate(out var isvalid);
            if (isvalid) return true;
            var message = string.Join(Environment.NewLine, errors.Select(x => x.ErrorMessage));
            lbStatus.Text = message;
            return false;
        }

        private List<MaskedTextBox> maskedBoxes = null;
        private static readonly UserPasswordChangeService changeService
            = SessionPersistenceContainer
            .GetContainer
            .GetInstance<UserPasswordChangeService>();

    }
}
