using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Enumerations;
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
        public bool Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;
                if (string.IsNullOrEmpty(Landing)) return false;
                if (RetryCount <= 0) return false;
                SessionUtil.Initialize();
                var payload = new { userName = username, password };
                using var client = new HttpClient();
                var response = http.PostAsJson<object, AuthenicationResponseDto>(client, Landing, payload);
                if (response == null || response.Id < 0)
                {
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
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

        }
        private static string Landing
        {
            get
            {
                if (landing != null) return landing; 
                var webid = (int)WebLandingName.LegacyLogin;
                var service = new CountyCodeService();
                landing = service.GetWebAddress(webid);
                return landing;
            }
        }

        private static string landing = null;
        private readonly IHttpService http;
    }
}
