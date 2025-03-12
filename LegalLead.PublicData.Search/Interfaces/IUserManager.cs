using LegalLead.PublicData.Search.Models;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface IUserManager
    {
        AdminDbResponse FetchData(AdminDbRequest request);
        void BindGrid(DataGridView gridView, AdminDbResponse response);
    }
}
