using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;
using Thompson.RecordSearch.Utility.Tools;

namespace LegalLead.PublicData.Search.Helpers
{
    
    public class AuthenicationService : IAuthenicationService
    {
        public AuthenicationService(IHttpService service)
        {
            http = service;
        }
        public int RetryCount { get; private set; } = 5;
        public async Task<bool> LoginAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;
            if (string.IsNullOrEmpty(Landing)) return false;
            if (RetryCount <= 0) return false;
            SessionUtil.Initialize();
            var payload = new { userName = username, password };
            using var client = new HttpClient();
            var response = await http.PostAsJsonAsync<object, AuthenicationResponseDto>(client, Landing, payload);
            if (response == null || response.Id < 0) {
                RetryCount--;
                return false; 
            }
            var mapped = UserPermissionHelper.GetPermissions(response.Id);
            if (mapped != null)
            {
                var json = JsonConvert.SerializeObject(mapped);
                SessionUtil.Write(json);
            }
            return true;

        }
        private static string Landing
        {
            get
            {
                if (landing != null) return landing;
                var service = new CountyCodeService();
                landing = service.GetWebAddress(1);
                return landing;
            }
        }
        
        private static string landing = null;
        private readonly IHttpService http;
    }
}
