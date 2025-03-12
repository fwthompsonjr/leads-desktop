using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Helpers
{
    public class UserManagerNonActive : BaseUserManager, IUserManager
    {
        protected override string MethodName => "None";

        public override AdminDbResponse FetchData(AdminDbRequest request)
        {
            return new()
            {
                IsSuccess = true,
                MethodName = MethodName,
                Message = "Process is not defined"
            };
        }

        public void BindGrid(DataGridView gridView, AdminDbResponse response)
        {
            response.MethodName = "Undefined";
            response.Message = "Process is not defined";
            response.IsSuccess = false;
            BindResponse(gridView, response);
        }
    }
}
