using LegalLead.PublicData.Search.Interfaces;
using System;
using System.Net.Http;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Helpers
{
    public class HccCountingService : IHccCountingService
    {
        public HccCountingService(IHttpService http)
        {
            httpService = http;
        }
        private readonly IHttpService httpService;
        public int Count(DateTime date)
        {
            var payload = new { FilingDate = date };
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            var data = httpService.PostAsJson<object, object>(client, RemoteAddress, payload);
            if (data == null) return 0;
            var people = data.ToJsonString().ToInstance<RemoteCountDto>();
            return people?.RecordCount.GetValueOrDefault() ?? 0;
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
                string.IsNullOrEmpty(model.CountUrl))
                throw new NullReferenceException();
            return string.Concat(model.Url, model.CountUrl);
        }

        private sealed class RemoteCountDto
        {
            public string Id { get; set; } = string.Empty;
            public int? RecordCount { get; set; } = null;
        }

    }
}