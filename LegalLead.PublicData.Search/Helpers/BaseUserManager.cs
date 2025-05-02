using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using LegalLead.PublicData.Search.Util;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Helpers
{
    public abstract class BaseUserManager
    {
        protected abstract string MethodName { get; }
        public virtual AdminDbResponse FetchData(AdminDbRequest request)
        {
            request.MethodName = MethodName;
            var response = dbHelper.Admin(request);
            if (response != null)
            {
                if (response.IsSuccess) return response;
                if (!string.IsNullOrWhiteSpace(response.Message)) return response;
                response.Message = "API response returned error. Please verify values and resubmit";
                return response;
            }
            return new AdminDbResponse()
            {
                IsSuccess = false,
                MethodName = MethodName,
                Message = "API response was empty. Please verify values and resubmit"
            };
        }
        public virtual void SaveGrid(DataGridView gridView)
        {

        }
        protected static void BindResponse(DataGridView gridView, AdminDbResponse response)
        {
            var data = new[] { response };
            if (gridView.ColumnCount > 0)
            {
                gridView.Columns.Clear();
            }
            gridView.DataSource = data;
            gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridView.Columns["MethodName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            gridView.Columns["MethodName"].Width = 200; // Set a fixed width for the first column
            gridView.Columns["IsSuccess"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            gridView.Columns["IsSuccess"].Width = 100; // Set a fixed width for the second column
            gridView.Columns["Message"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            for (var col = 0; col < gridView.Columns.Count; col++)
            {
                gridView.Columns[col].ReadOnly = true;
            }
        }



        protected static readonly IRemoteDbHelper dbHelper
            = ActionSettingContainer.GetContainer.GetInstance<IRemoteDbHelper>();
    }
}
