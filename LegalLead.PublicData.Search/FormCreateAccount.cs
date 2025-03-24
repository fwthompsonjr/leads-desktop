using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FormCreateAccount : Form
    {
        private readonly RegisterAccountModel userModel;
        public FormCreateAccount()
        {
            InitializeComponent();
            userModel = new RegisterAccountModel();
            btnSubmit.Click += BtnSubmit_Click;
            Load += FormCreateAccount_Load;
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (!btnSubmit.Enabled) return;
            try
            {
                btnSubmit.Enabled = false;
                lbStatus.Text = "Validing input";
                // Update model with current values from TextBox controls
                this.BindingContext[userModel].EndCurrentEdit();
                // Validate the model
                if (ModelValidator.Validate(userModel, out List<ValidationResult> results))
                {
                    lbStatus.Text = "Processing input";
                }
                else
                {
                    string errorMessages = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
                    lbStatus.Text = $"Validation failed:\n{errorMessages}";
                }
            }
            finally
            {
                btnSubmit.Enabled = true;
            }
        }

        private void FormCreateAccount_Load(object sender, EventArgs e)
        {
            BindModelToControls();
        }

        private void BindModelToControls()
        {
            txUserName.DataBindings.Add("Text", userModel, "UserName");
            txEmail.DataBindings.Add("Text", userModel, "Email");
            tbxPwd.DataBindings.Add("Text", userModel, "Password");
            tbxConfirmPwd.DataBindings.Add("Text", userModel, "ConfirmPassword");
        }
        private static class ModelValidator
        {
            public static bool Validate(object model, out List<ValidationResult> results)
            {
                ValidationContext context = new(model, null, null);
                results = [];
                return Validator.TryValidateObject(model, context, results, true);
            }
        }
    }
}
