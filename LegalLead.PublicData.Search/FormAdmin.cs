using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
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
            gridUsers.CellContentClick += GridUsers_CellContentClick;
            InitializeChildPanelControls();
            InitializeParentMenuButtons();
        }

        private void GridUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            lock (harmony)
            {
                var isTagSet = false;
                try
                {
                    if (e.RowIndex < 0) return;
                    if (sender is not DataGridView gridView) return;
                    if (gridView.Columns[e.ColumnIndex].Name != "EditAccount") return;
                    if (gridView.DataSource is not List<GetAccountsResponse> accounts) return;
                    var item = accounts[e.RowIndex];
                    cboUserAction.Tag = item;
                    isTagSet = true;
                }
                finally
                {
                    if (!isTagSet) cboUserAction.Tag = null;
                }
            }

        }

        private void InitializeChildPanelControls()
        {
            var items = GetAccountActions();
            cboUserAction.DataSource = items;
            cboUserAction.DisplayMember = "DisplayName";
            cboUserAction.ValueMember = "Id";
            cboUserAction.SelectedIndex = 0;
            cboUserAction.SelectedIndexChanged += CboUserAction_SelectedIndexChanged;
        }

        private void InitializeParentMenuButtons()
        {
            var models = GetOperations();
            if (models.Length > 0)
            {
                foreach (var model in models)
                {
                    var tsItem = new ToolStripButton
                    {
                        Text = model.DisplayName,
                        Padding = new Padding(2),
                        Tag = model,
                    };
                    tsItem.Click += TsItem_Click;
                    toolStrip.Items.Add(tsItem);
                    if (model.Method == UserManagementMethod.GetAccounts)
                    {
                        tsItem.PerformClick();
                    }
                }
            }
        }

        private void CboUserAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (harmony)
            {
                if (cboUserAction.SelectedItem is not UserManagementMethodModel model) return;
                if (cboUserAction.Tag is not GetAccountsResponse rsp || model.Id == 1000)
                {
                    grid.Visible = false;
                    return;
                }
                var api = UserManagementContainer.GetContainer.GetInstance<IUserManager>(model.Name);
                var response = api.FetchData(new Models.AdminDbRequest { UserId = rsp.Id });
                api.BindGrid(grid, response);
                grid.Visible = true;
            }
        }

        private void TsItem_Click(object sender, EventArgs e)
        {
            if (sender is not ToolStripButton btn) return;
            if (btn.Tag is not UserManagementMethodModel model) return;
            var api = UserManagementContainer.GetContainer.GetInstance<IUserManager>(model.Name);
            var response = api.FetchData(new Models.AdminDbRequest());
            api.BindGrid(gridUsers, response);
            tsContext.Text = model.DisplayName;
            tsPosition.Text = string.Empty;
            splitContainer.Panel2Collapsed = model.Method != UserManagementMethod.GetAccounts;
            if (splitContainer.Panel2Collapsed) return;
            splitContainer.SplitterDistance = 200;
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
            foreach (var process in processes)
            {
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
        private static readonly object harmony = new();
    }
}
