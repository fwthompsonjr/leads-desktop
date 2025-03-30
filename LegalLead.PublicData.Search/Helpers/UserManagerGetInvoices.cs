using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System.Collections.Generic;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Helpers
{
    public class UserManagerGetInvoices : BaseUserManager, IUserManager
    {
        protected override string MethodName => "GetInvoice";

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
                var rows = response.Message.ToInstance<List<GetInvoiceResponse>>() ?? [];
                gridView.DataSource = rows;

                gridView.Columns["Id"].Visible = false;
                gridView.Columns["LeadUserId"].Visible = false;
                gridView.Columns["RequestId"].Visible = false;
                gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                gridView.Columns["InvoiceNbr"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                gridView.Columns["InvoiceNbr"].Width = 250; // Set a fixed width for the first column
                gridView.Columns["InvoiceUri"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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