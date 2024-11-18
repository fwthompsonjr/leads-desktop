using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Enumerations;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;
namespace LegalLead.PublicData.Search.Helpers
{

    public class ApiAuthenicationService : IAuthenicationService
    {
        public ApiAuthenicationService(IHttpService service)
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
                var response = http.PostAsJson<object, ApiResponseModel>(client, Landing, payload);
                if (response == null || string.IsNullOrEmpty(response.Token))
                {
                    RetryCount--;
                    return false;
                }
                var mapped = GetModel(response.Token, out var _);
                if (mapped != null)
                {
                    var json = JsonConvert.SerializeObject(mapped);
                    SessionUtil.Write(json);
                }
                return (mapped != null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

        }

        public static LeadUserSecurityBo GetModel(string token, out DateTime? expirationDate)
        {
            expirationDate = null;
            try
            {
                var position = DateTime.UtcNow.ToString("s").Length;
                var data = Convert.FromBase64String(token);
                var expiry = encoding.GetString(data.Take(position).ToArray());
                var serialized = encoding.GetString(data.Skip(position).ToArray());
                var model = serialized.ToInstance<LeadUserSecurityBo>();
                if (model == null) return null;
                if (!DateTime.TryParseExact(expiry, "s", enUS, DateTimeStyles.None, out var dateValue)) return null;
                expirationDate = dateValue;
                return model;
            }
            catch
            {
                return null;
            }
        }

        private static string Landing
        {
            get
            {
                if (landing != null) return landing;
                var webid = (int)WebLandingName.Login;
                var service = new CountyCodeService();
                landing = service.GetWebAddress(webid);
                return landing;
            }
        }

        private static string landing = null;
        private readonly IHttpService http;

        private static readonly Encoding encoding = Encoding.UTF8;
        private static readonly CultureInfo enUS = new("en-US");
    }
}