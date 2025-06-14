using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FormAdmin : Form
    {

        private EventHandler<GetAccountsResponse> OnTagChanged;

        public FormAdmin()
        {
            InitializeComponent();
            splitContainer.Panel2Collapsed = true;
            gridUsers.CellContentClick += GridUsers_CellContentClick;
            grid.CellValueChanged += Grid_CellValueChanged;

            InitializeChildPanelControls();
            InitializeParentMenuButtons();
            OnTagChanged += TagModified;
            buttonSaveChanges.Click += ButtonSaveChanges_Click;
            testToolStripMenuItem.Click += MenuChangeBillingType_Click;
            prodToolStripMenuItem.Click += MenuChangeBillingType_Click;
        }

        private void MenuChangeBillingType_Click(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem menuItem) return;
            if (menuItem.Checked) return;
            if (grid.DataSource is not List<BillTypeHistoryModel> models || models.Count == 0) return;
            var latestModel = models[0];
            if (menuItem.Tag is not string billingCode) return;
            var userId = latestModel.UserId;
            invoiceReader.SetBillingCode(userId, billingCode);
            CboUserAction_SelectedIndexChanged(null, null);
        }

        private void Grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (grid.IsCurrentCellDirty)
            {
                grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void ButtonSaveChanges_Click(object sender, EventArgs e)
        {
            if (!buttonSaveChanges.Enabled) return;
            lock (harmony)
            {
                buttonSaveChanges.Enabled = false;
                try
                {
                    if (cboUserAction.SelectedItem is not UserManagementMethodModel model) return;
                    if (cboUserAction.Tag is not GetAccountsResponse rsp || model.Id == 1000) return;

                    var api = UserManagementContainer.GetContainer.GetInstance<IUserManager>(model.Name);
                    api.SaveGrid(grid);

                    var response = api.FetchData(new Models.AdminDbRequest { UserId = rsp.Id });
                    api.BindGrid(grid, response);

                }
                finally
                {
                    buttonSaveChanges.Enabled = true;
                }
            }
        }

        private void TagModified(object sender, GetAccountsResponse e)
        {
            if (cboUserAction.Tag == null)
            {
                lbUserName.Text = "-";
                return;
            }
            lbUserName.Text = e.UserName;
            if (cboUserAction.SelectedIndex == 0) cboUserAction.SelectedIndex = 1;
            CboUserAction_SelectedIndexChanged(null, null);
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
                    foreach (DataGridViewRow row in gridView.Rows)
                    {
                        if (row.Index < 0) continue;
                        row.Selected = row.Index == e.RowIndex;
                    }
                    var item = accounts[e.RowIndex];
                    cboUserAction.Tag = item;
                    isTagSet = true;
                    OnTagChanged?.Invoke(null, item);
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
                tsButtonSetBilling.Visible = false;
                if (cboUserAction.SelectedItem is not UserManagementMethodModel model) return;
                if (cboUserAction.Tag is not GetAccountsResponse rsp || model.Id == 1000)
                {
                    grid.Visible = false;
                    buttonSaveChanges.Visible = false;
                    return;
                }
                var api = UserManagementContainer.GetContainer.GetInstance<IUserManager>(model.Name);
                var response = api.FetchData(new Models.AdminDbRequest { UserId = rsp.Id });
                api.BindGrid(grid, response);
                grid.Visible = true;
                var allowEdits = new int[] {
                    (int)UserManagementMethod.GetCounty,
                    (int)UserManagementMethod.GetProfile };
                buttonSaveChanges.Visible = allowEdits.Contains(model.Id);
                int billingId = (int)UserManagementMethod.GetBillCode;
                if (model.Id != billingId) return;
                tsButtonSetBilling.Visible = true;
                if (grid.DataSource is not List<BillTypeHistoryModel> models || models.Count == 0) return;
                var latestModel = models[0];
                testToolStripMenuItem.Checked = latestModel.KeyValue.Equals("Test", StringComparison.OrdinalIgnoreCase);
                prodToolStripMenuItem.Checked = !testToolStripMenuItem.Checked;
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
                var aa = b.Id.CompareTo(a.Id);
                if (aa != 0) return aa;
                return (a.DisplayName.CompareTo(b.DisplayName));
            });
            return [.. list];
        }
        private enum UserManagementMethod
        {
            GetAccounts = 1,
            GetPricing = 2,
            GetCounty = 150,
            GetProfile = 140,
            GetSearch = 120,
            GetInvoice = 110,
            GetBillCode = 160,
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
                UserManagementMethod.GetBillCode => "Billing",
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
        private static readonly IRemoteInvoiceHelper invoiceReader = ActionSettingContainer
        .GetContainer
        .GetInstance<IRemoteInvoiceHelper>();
    }
}
