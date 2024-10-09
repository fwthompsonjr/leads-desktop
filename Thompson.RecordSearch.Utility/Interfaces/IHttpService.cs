using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Interfaces
{
    public interface IHttpService
    {
        Task<TItem> PostAsJsonAsync<T, TItem>(HttpClient client, string webaddress, T value, CancellationToken cancellationToken = default);
    }
}
