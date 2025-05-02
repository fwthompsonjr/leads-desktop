using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using LegalLead.PublicData.Search.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;

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
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
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
                    var list = GetUsers();
                    if (list == null)
                    {
                        lbStatus.Text = "User validation failed";
                        return;
                    }
                    if (list.Exists(x => x.UserName.Equals(userModel.UserName, oic)))
                    {
                        lbStatus.Text = $"Invalid username. {userModel.UserName}. Entry already exists with this user name";
                        return;
                    }
                    if (list.Exists(x => x.Email.Equals(userModel.Email, oic)))
                    {
                        lbStatus.Text = $"Invalid email. {userModel.Email}. Entry already exists with this email.";
                        return;
                    }

                    dbHelper.RegisterAccount(userModel);
                    list = GetUsers();
                    var isCreated = list.Exists(x => x.UserName.Equals(userModel.UserName, StringComparison.OrdinalIgnoreCase));
                    var statusMessage = isCreated ? "Account registration completed" : "Error processing request";
                    lbStatus.Text = statusMessage;
                    return;
                }
                string errorMessages = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
                lbStatus.Text = $"Validation failed:\n{errorMessages}";
            }
            finally
            {
                btnSubmit.Enabled = true;
            }
        }

        private void FormCreateAccount_Load(object sender, EventArgs e)
        {
            BindModelToControls();
            txUserName.Focus();
        }

        private void BindModelToControls()
        {
            txUserName.DataBindings.Add("Text", userModel, "UserName");
            txEmail.DataBindings.Add("Text", userModel, "Email");
            tbxPwd.DataBindings.Add("Text", userModel, "Password");
            tbxConfirmPwd.DataBindings.Add("Text", userModel, "ConfirmPassword");
        }
        private static List<GetAccountsResponse> GetUsers()
        {

            var api = UserManagementContainer.GetContainer.GetInstance<IUserManager>("GetAccounts");
            var response = api.FetchData(new Models.AdminDbRequest());
            if (!response.IsSuccess) return null;
            return response.Message.ToInstance<List<GetAccountsResponse>>();

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


        private static readonly IRemoteDbHelper dbHelper
            = ActionSettingContainer.GetContainer.GetInstance<IRemoteDbHelper>();
    }
}
