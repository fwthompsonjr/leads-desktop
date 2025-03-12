using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System.Collections.Generic;
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
                    gridView.Columns?.Clear();
                }
                if (!response.IsSuccess)
                {
                    BindResponse(gridView, response);
                    return;
                }
                var rows = response.Message.ToInstance<List<GetCountyResponse>>() ?? [];
                gridView.DataSource = rows;

                gridView.Columns["Id"].Visible = false;
                gridView.Columns["CountyId"].Visible = false;
                gridView.Columns["RwId"].Visible = false;
                gridView.Columns["LeadUserId"].Visible = false;
                gridView.Columns["Phrase"].Visible = false;
                gridView.Columns["Vector"].Visible = false;
                gridView.Columns["Token"].Visible = false;
                gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                gridView.Columns["CountyName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                gridView.Columns["CountyName"].Width = 250; // Set a fixed width for the first column
                gridView.Columns["MonthlyUsage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                gridView.Columns["CreateDate"].Width = 150; // Set a fixed width for the last column
                for (var col = 0; col < gridView.Columns.Count; col++)
                {
                    var name = gridView.Columns[col].Name;
                    gridView.Columns[col].ReadOnly = !name.Equals("MonthlyUsage");
                }

            }
            finally
            {
                gridView.Refresh();
            }
        }
    }
}
