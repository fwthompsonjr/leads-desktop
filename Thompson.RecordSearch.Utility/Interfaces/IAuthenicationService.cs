using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Interfaces
{
    public interface IAuthenicationService
    {
        Task<bool> LoginAsync(string username, string password);
    }
}
