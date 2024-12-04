using LegalLead.PublicData.Search.Interfaces;
using System;
using System.Net.Http;
using System.Text;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Helpers
{
    public class HccWritingService : IHccWritingService
    {
        public HccWritingService(IHttpService http)
        {
            httpService = http;
        }
        private readonly IHttpService httpService;
        public void Write(string csvdata)
        {
            var dat = Encoding.UTF8.GetBytes(csvdata);
            var conversion = Convert.ToBase64String(dat);
            var payload = new { Content = conversion };
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _ = httpService.PostAsJson<object, object>(client, RemoteAddress, payload);
        }
        private static string RemoteAddress
        {
            get
            {
                if (!string.IsNullOrEmpty(_remoteAddress)) return _remoteAddress;
                _remoteAddress = GetRemoteAddress();
                return _remoteAddress;
            }
        }
        private static string _remoteAddress = string.Empty;
        private static string GetRemoteAddress()
        {
            var model = HccConfigurationModel.GetModel().RemoteModel;
            if (string.IsNullOrEmpty(model.Url) ||
                string.IsNullOrEmpty(model.FetchUrl))
                throw new NullReferenceException();
            return string.Concat(model.Url, model.PostUrl);
        }
    }
}