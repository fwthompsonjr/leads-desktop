using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Helpers
{
    public class UserManagerGetSearch : BaseUserManager, IUserManager
    {
        protected override string MethodName => "GetSearch";

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
                var rows = response.Message.ToInstance<List<GetSearchResponse>>() ?? [];
                gridView.DataSource = rows;

                gridView.Columns["Id"].Visible = false;
                gridView.Columns["LeadUserId"].Visible = false;
                gridView.Columns["CountyId"].Visible = false;
                gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                for (var col = 0; col < gridView.Columns.Count; col++)
                {
                    var name = gridView.Columns[col].Name;
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