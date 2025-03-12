using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FormAdmin : Form
    {
        public FormAdmin()
        {
            InitializeComponent();
            splitContainer.Panel2Collapsed = true;
            var models = GetOperations();
            if (models.Length > 0) {
                foreach (var model in models) {
                    var tsItem = new ToolStripButton
                    {
                        Text = model.DisplayName,
                        Padding = new Padding(2),
                        Tag = model,
                    };
                    tsItem.Click += TsItem_Click;
                    toolStrip.Items.Add(tsItem);
                }
            }
            var items = GetAccountActions();
            cboUserAction.DataSource = items;
            cboUserAction.DisplayMember = "DisplayName";
            cboUserAction.ValueMember = "Id";
            cboUserAction.SelectedIndex = 0;
            cboUserAction.SelectedIndexChanged += CboUserAction_SelectedIndexChanged;
            CboUserAction_SelectedIndexChanged(null, null);
        }

        private void CboUserAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboUserAction.SelectedItem is not UserManagementMethodModel model) return;
            grid.Visible = model.Id != 1000;
        }

        private void TsItem_Click(object sender, EventArgs e)
        {
            if (sender is not ToolStripButton btn) return;
            if (btn.Tag is not UserManagementMethodModel model) return;
            
            tsContext.Text = model.DisplayName;
            tsPosition.Text = string.Empty;
            splitContainer.Panel2Collapsed = model.Method != UserManagementMethod.GetAccounts;
        }

        private void TsReturn_Click(object sender, EventArgs e)
        {
            if (Program.mainForm == null) return;
            Program.mainForm.mnuViewUsers.PerformClick();
        }
        private static UserManagementMethodModel[] GetOperations()
        {
            var processes = Enum.GetValues<UserManagementMethod>();
            var list = new List<UserManagementMethodModel>();
            foreach (var process in processes) {
                var model = new UserManagementMethodModel(process);
                if (model.Id < 100) list.Add(model);
            }
            return [.. list];
        }

        private static UserManagementMethodModel[] GetAccountActions()
        {
            var processes = Enum.GetValues<UserManagementMethod>();
            var list = new List<UserManagementMethodModel>();
            foreach (var process in processes)
            {
                var model = new UserManagementMethodModel(process);
                if (model.Id == 1000 || model.Id > 100 && model.Id < 200) list.Add(model);
            }
            list.Sort((a, b) =>
            {
                if (a.Id == 1000) return -1;
                return (a.Id.CompareTo(b.Id));
            });
            return [.. list];
        }
        private enum UserManagementMethod
        {
            GetAccounts = 1,
            GetPricing = 2,
            GetCounty = 101,
            GetProfile = 102,
            GetInvoice = 103,
            GetSearch = 104,
            UpdateProfile = 201,
            UpdateUsageLimit = 202,
            None = 1000,
        }

        private sealed class UserManagementMethodModel(UserManagementMethod method)
        {
            public int Id { get; private set; } = (int)method;
            public string Name { get; set; } = Enum.GetName(method);
            public string DisplayName { get; set; } = method switch
            {
                UserManagementMethod.GetAccounts => "Accounts",
                UserManagementMethod.GetPricing => "Pricing",
                UserManagementMethod.GetCounty => "Counties",
                UserManagementMethod.GetProfile => "Profile",
                UserManagementMethod.GetSearch => "Searches",
                UserManagementMethod.GetInvoice => "Invoices",
                UserManagementMethod.UpdateProfile => "Update Profile",
                UserManagementMethod.UpdateUsageLimit => "Update Search Limit",
                _ => "N/A"
            };
            public UserManagementMethod Method { get; set; } = method;
        }
    }
}
