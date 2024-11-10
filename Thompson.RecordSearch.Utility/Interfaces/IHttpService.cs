using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Interfaces
{
    public interface IHttpService
    {
        TItem PostAsJson<T, TItem>(HttpClient client, string webaddress, T value, CancellationToken cancellationToken = default);
        Task<TItem> PostAsJsonAsync<T, TItem>(HttpClient client, string webaddress, T value, CancellationToken cancellationToken = default);
    }
}
