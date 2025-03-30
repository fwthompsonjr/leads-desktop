using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Helpers
{
    public class UserManagerGetCounty : BaseUserManager, IUserManager
    {
        protected override string MethodName => "GetCounty";

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
                var rows = response.Message.ToInstance<List<GetCountyResponse>>() ?? [];
                gridView.DataSource = rows;
                gridView.Tag = rows.ToJsonString();

                gridView.Columns["Id"].Visible = false;
                gridView.Columns["CountyId"].Visible = false;
                gridView.Columns["RwId"].Visible = false;
                gridView.Columns["LeadUserId"].Visible = false;
                gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                gridView.Columns["CountyName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                gridView.Columns["CountyName"].Width = 250; // Set a fixed width for the first column
                gridView.Columns["MonthlyUsage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                gridView.Columns["UserName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                gridView.Columns["CreateDate"].Width = 150; // Set a fixed width for the last column
                var editables = new List<string> { "MonthlyUsage" };
                for (var col = 0; col < gridView.Columns.Count; col++)
                {
                    var name = gridView.Columns[col].Name;
                    gridView.Columns[col].ReadOnly = !editables.Contains(name);
                }
                var rightAlignment = new List<string> { "MonthlyUsage", "CreateDate" };
                rightAlignment.ForEach(columnName =>
                {
                    var col = gridView.Columns[columnName];
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    col.DefaultCellStyle.Padding = new Padding(0, 0, 4, 0);
                });
            }
            finally
            {
                gridView.Refresh();
            }
        }


        public override void SaveGrid(DataGridView gridView)
        {
            if (gridView.DataSource is not List<GetCountyResponse> changes) return;
            if (gridView.Tag is not string json) return;
            var source = json.ToInstance<List<GetCountyResponse>>();
            if (source == null) return;
            var worklist = new List<GetCountyResponse>();
            changes.ForEach(x => {
                if (IsItemChanged(x, source[changes.IndexOf(x)])) worklist.Add(x);
            });
            if (worklist.Count == 0) return;
            AdminDbRequest saveRequest = new()
            {
                MethodName = "UpdateUsageLimit",
                Payload = worklist.ToJsonString(),
                UserId = worklist[0].LeadUserId,
            };
            dbHelper.Admin(saveRequest);

            Debug.WriteLine("Found {0} records changed", worklist.Count);
        }

        private static bool IsItemChanged(GetCountyResponse userData, GetCountyResponse current)
        {
            return userData.MonthlyUsage.GetValueOrDefault() != current.MonthlyUsage.GetValueOrDefault();
        }
    }
}
