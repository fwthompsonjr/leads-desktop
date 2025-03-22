using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Helpers
{
    public class UserManagerGetProfile : BaseUserManager, IUserManager
    {
        protected override string MethodName => "GetProfile";

        public void BindGrid(DataGridView gridView, AdminDbResponse response)
        {
            try
            {
                if (gridView.DataSource != null)
                {
                    gridView.DataSource = null;
                    gridView.Columns?.Clear();
                }
                if (!response.IsSuccess)
                {
                    BindResponse(gridView, response);
                    return;
                }
                var rows = response.Message.ToInstance<List<GetProfileResponse>>() ?? [];
                rows.ForEach(r =>
                {
                    if (string.IsNullOrEmpty(r.Id)) r.Id = Guid.NewGuid().ToString("D");
                });
                gridView.DataSource = rows;

                gridView.Columns["Id"].Visible = false;
                gridView.Columns["UserId"].Visible = false;
                gridView.Columns["ProfileMapId"].Visible = false;
                gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                gridView.Columns["KeyName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                gridView.Columns["KeyName"].Width = 250; // Set a fixed width for the first column
                gridView.Columns["KeyValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                for (var col = 0; col < gridView.Columns.Count; col++)
                {
                    var name = gridView.Columns[col].Name;
                    gridView.Columns[col].ReadOnly = !name.Equals("KeyValue");
                }

            }
            finally
            {
                gridView.Refresh();
            }
        }
    }
}
