using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Helpers
{
    public class UserManagerGetAccounts : BaseUserManager, IUserManager
    {
        protected override string MethodName => "GetAccounts";

        public void BindGrid(DataGridView gridView, AdminDbResponse response)
        {
            try
            {
                if (gridView.DataSource != null) {
                    gridView.DataSource = null;
                    gridView.Columns?.Clear();
                }
                if (!response.IsSuccess)
                {
                    BindResponse(gridView, response);
                    return;
                }
                var rows = response.Message.ToInstance<List<GetAccountsResponse>>() ?? [];
                gridView.DataSource = rows;
                DataGridViewButtonColumn buttonColumn = new()
                {
                    HeaderText = "Edit Account",
                    Text = "Edit",
                    UseColumnTextForButtonValue = true,
                    Name = "EditAccount",
                    DataPropertyName = "EditAccount",
                };
                gridView.Columns.Add(buttonColumn);
                gridView.Columns["Id"].Visible = false;
                gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                gridView.Columns["Email"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                gridView.Columns["Email"].Width = 250; // Set a fixed width for the first column
                gridView.Columns["UserName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                gridView.Columns["EditAccount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                gridView.Columns["EditAccount"].Width = 80; // Set a fixed width for the last column
                for (var col = 0; col < gridView.Columns.Count; col++)
                {
                    gridView.Columns[col].ReadOnly = true;
                }

            }
            finally
            {
                gridView.Refresh();
            }
        }
    }
}
