using System.Net.Http;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Enumerations;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Helpers
{
    public class SessionUsagePersistence
    {
        public SessionUsagePersistence(IHttpService service)
        {
            http = service;
        }

        public bool IncementUsage(string userId, string county, int recordCount)
        {
            if (string.IsNullOrWhiteSpace(userId) || recordCount < 0) return false;
            if (string.IsNullOrEmpty(Landing)) return false;
            var payload = new { UserName = userId, CountyName = county, MonthlyUsage = recordCount };
            var response = GetHttpRespone<object, object>(payload, Landing);
            return response != null;
        }

        protected T2 GetHttpRespone<T1, T2>(T1 payload, string address)
        {
            var token = SessionUtil.Read();
            if (string.IsNullOrEmpty(token)) return default;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("LEAD_IDENTITY", token);
            var response = http.PostAsJson<T1, T2>(client, address, payload);
            return response;
        }
        private static string Landing
        {
            get
            {
                if (landing != null) return landing;
                var webid = (int)WebLandingName.UsageIncrement;
                var service = new CountyCodeService();
                landing = service.GetWebAddress(webid);
                return landing;
            }
        }

        private static string landing = null;
        protected readonly IHttpService http;
    }
}
