using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Interfaces;

namespace Thompson.RecordSearch.Utility.Classes
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "<Pending>")]
    public class HttpService : IHttpService
    {
        public async Task<TItem> PostAsJsonAsync<T, TItem>(HttpClient client, string webaddress, T value, CancellationToken cancellationToken = default)
        {
            var response = await Task.Run(() =>
            {
                return PostAsJson<T, TItem>(client, webaddress, value, cancellationToken);
            });
            return response;
        }

        public TItem PostAsJson<T, TItem>(HttpClient client, string webaddress, T value, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (string.IsNullOrWhiteSpace(webaddress)) throw new ArgumentNullException(nameof(webaddress));
            if (!Uri.TryCreate(webaddress, UriKind.Absolute, out var _)) throw new ArgumentOutOfRangeException(nameof(webaddress));
            try
            {
                client.Timeout = TimeSpan.FromSeconds(90);
                using (var payload = GetContent(value))
                {
                    var response = client.PostAsync(webaddress, payload).GetAwaiter().GetResult();
                    if (!response.IsSuccessStatusCode) return default;
                    var content = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<TItem>(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }

        public string GetFromJsonPost<T>(HttpClient client, string webaddress, T value, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (string.IsNullOrWhiteSpace(webaddress)) throw new ArgumentNullException(nameof(webaddress));
            if (!Uri.TryCreate(webaddress, UriKind.Absolute, out var _)) throw new ArgumentOutOfRangeException(nameof(webaddress));
            try
            {
                client.Timeout = TimeSpan.FromSeconds(90);
                using (var payload = GetContent(value))
                {
                    var response = client.PostAsync(webaddress, payload).GetAwaiter().GetResult();
                    if (!response.IsSuccessStatusCode) return string.Empty;
                    var content = response.Content.ReadAsStringAsync().Result;
                    return content;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        private static ByteArrayContent GetContent(object payload)
        {
            var content = JsonConvert.SerializeObject(payload);
            var buffer = Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return byteContent;
        }

    }
}
