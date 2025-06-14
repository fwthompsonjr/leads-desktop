using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System.Collections.Generic;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Helpers
{
    public class UserManagerGetBillTypeHistory : BaseUserManager, IUserManager
    {
        protected override string MethodName => "GetBillCode";

        public void BindGrid(DataGridView gridView, AdminDbResponse response)
        {
            try
            {
                if (gridView.DataSource != null)
                {
                    gridView.DataSource = null;
                    gridView.Tag = null;
                    gridView.Columns?.Clear();
                }
                if (!response.IsSuccess)
                {
                    BindResponse(gridView, response);
                    return;
                }
                var rows = response.Message.ToInstance<List<BillTypeHistoryModel>>() ?? [];
                gridView.DataSource = rows;
                gridView.Tag = rows.ToJsonString();
                gridView.Columns["Id"].Visible = false;
                gridView.Columns["UserPermissionId"].Visible = false;
                gridView.Columns["UserId"].Visible = false;
                gridView.Columns["PermissionMapId"].Visible = false;
                gridView.Columns["GroupId"].Visible = false;
                gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                gridView.Columns["KeyName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                gridView.Columns["KeyName"].Width = 400; // Set a fixed width for the first column
                gridView.Columns["CreateDate"].Width = 150; // Set a fixed width for the last column
                var rightAlignment = new List<string> { "CreateDate" };
                rightAlignment.ForEach(columnName =>
                {
                    var col = gridView.Columns[columnName];
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    col.DefaultCellStyle.Padding = new Padding(0, 0, 4, 0);
                });
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
        public override AdminDbResponse FetchData(AdminDbRequest request)
        {
            var data = dbHelper.FetchBillTypeHistory(request.UserId);
            var rows = data.ToJsonString();
            return new AdminDbResponse
            {
                IsSuccess = true,
                MethodName = this.MethodName,
                Message = rows
            };
        }
    }
}