using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Interfaces;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class HttpService : IHttpService
    {
        public async Task<TItem> PostAsJsonAsync<T, TItem>(HttpClient client, string webaddress, T value, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (string.IsNullOrWhiteSpace(webaddress)) throw new ArgumentNullException(nameof(webaddress));
            if (!Uri.TryCreate(webaddress, UriKind.Absolute, out var uri)) throw new ArgumentOutOfRangeException(nameof(webaddress));
            try
            {
                client.Timeout = TimeSpan.FromSeconds(90);
                using (var payload = GetContent(value))
                {
                    var response = await client.PostAsync(uri, payload).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode) return default;
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<TItem>(content);
                }   
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return default;
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
