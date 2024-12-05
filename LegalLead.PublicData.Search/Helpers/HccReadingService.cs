using LegalLead.PublicData.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Helpers
{
    public class HccReadingService : IHccReadingService
    {
        public HccReadingService(IHttpService http)
        {
            httpService = http;
        }
        private readonly IHttpService httpService;
        public List<CaseItemDto> Find(DateTime date)
        {
            var found = new List<CaseItemDto>();
            var payload = new { FilingDate = date };
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            var data = httpService.PostAsJson<object, object>(client, RemoteAddress, payload);
            if (data == null) return found;
            var people = data.ToJsonString().ToInstance<List<RemotePersonDto>>();
            if (people == null || people.Count == 0) return found;
            var items = people.Select(s => new CaseItemDto
            {
                CaseNumber = s.CaseNumber,
                FileDate = ParseDate(s.CaseFileDate),
                CaseType = s.CurrentOffenseLiteral,
                CaseStatus = s.DefendantStatus,
                Court = s.CourtNumber,
                PartyName = s.DefendantName,
                Plaintiff = s.ComplainantName,
                CaseStyle = s.CurrentOffenseLiteral,
                Address = ParseAddress(s)
            });
            found.AddRange(items);
            return found;
        }

        private static string ParseDate(string originalDate)
        {
            if (string.IsNullOrWhiteSpace(originalDate)) return originalDate;
            if (!DateTime.TryParseExact(originalDate, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var date)) return originalDate;
            return date.ToString("d", CultureInfo.CurrentCulture);
        }
        private static string ParseAddress(RemotePersonDto source)
        {
            var city = string.IsNullOrWhiteSpace(source.DefendantCity) ? "" : $"{source.DefendantCity},";
            var items = new[]
            {
                source.DefendantStreetNumber,
                source.DefendantStreetName,
                source.DefendantApartmentNumber,
                city,
                source.DefendantState,
                source.DefendantZip
            }.ToList();
            items.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            return string.Join(" ", items);
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
            return string.Concat(model.Url, model.FetchUrl);
        }

        private sealed class RemotePersonDto
        {
            public string Id { get; set; } = string.Empty;
            public string CourtDivisionIndicator { get; set; } = string.Empty;
            public string CaseNumber { get; set; } = string.Empty;
            public string CaseFileDate { get; set; } = string.Empty;
            public string CourtNumber { get; set; } = string.Empty;
            public string CaseStatus { get; set; } = string.Empty;
            public string DefendantStatus { get; set; } = string.Empty;
            public string CurrentOffenseCode { get; set; } = string.Empty;
            public string CurrentOffenseLiteral { get; set; } = string.Empty;
            public string CurrentOffenseLevelAndDegree { get; set; } = string.Empty;
            public string DefendantName { get; set; } = string.Empty;
            public string DefendantStreetNumber { get; set; } = string.Empty;
            public string DefendantStreetName { get; set; } = string.Empty;
            public string DefendantApartmentNumber { get; set; } = string.Empty;
            public string DefendantCity { get; set; } = string.Empty;
            public string DefendantState { get; set; } = string.Empty;
            public string DefendantZip { get; set; } = string.Empty;
            public string ComplainantName { get; set; } = string.Empty;
            public DateTime? CreateDate { get; set; } = null;
        }

    }
}
