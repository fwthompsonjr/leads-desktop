using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Util
{
    public class UiDbInteractive : IWebInteractive
    {
        public UiDbInteractive(
            IWebInteractive interactive,
            IRemoteDbHelper db,
            int searchTypeId = 0,
            int caseTypeId = 0,
            int districtCourtId = 0,
            int districtSearchTypeId = 0)
        {
            _web = interactive;
            _dbsvc = db;
            WebRequest = new FindDbRequest
            {
                CountyId = _web.Parameters.Id,
                SearchDate = _web.StartDate,
                SearchTypeId = searchTypeId,
                CaseTypeId = caseTypeId,
                DistrictCourtId = districtCourtId,
                DistrictSearchTypeId = districtSearchTypeId
            };
            var list = sourceData.FindAll(x => x.Category == "browser");
            var lookups = new[] {
                new SettingLookupDto {
                    Name = "Database Search:",
                    DefaultValue = "true"
                },
                new SettingLookupDto {
                    Name = "Database Minimun Persistence:",
                    DefaultValue = "15"
                },
            }.ToList();
            foreach (var lookup in lookups)
            {
                var src = list.Find(x => x.Name == lookup.Name);
                lookup.Value = src?.Value ?? string.Empty;
            }
            var useDb = lookups[0].GetValue();
            var minDayInterval = lookups[1].GetValue();
            var bUseDb = bool.TryParse(useDb, out var bUse);
            var bMinDays = int.TryParse(minDayInterval, out var minInterval);
            UseRemoteDb = GetValueOrDefault(bUseDb, bUse, true);
            RemoteMinDayInterval = GetValueOrDefault(bMinDays, minInterval, 15);

        }
        private readonly bool UseRemoteDb;
        private readonly int RemoteMinDayInterval;
        private readonly FindDbRequest WebRequest;
        private readonly IRemoteDbHelper _dbsvc;
        private readonly IWebInteractive _web;
        public DateTime EndingDate
        {
            get => _web.EndingDate;
            set { _web.EndingDate = value; }
        }
        public WebNavigationParameter Parameters
        {
            get => _web.Parameters;
            set { _web.Parameters = value; }
        }
        public string Result
        {
            get => _web.Result;
            set { _web.Result = value; }
        }
        public DateTime StartDate
        {
            get => _web.StartDate;
            set { _web.StartDate = value; }
        }
        public Action<int, int, int> ReportProgress
        {
            get => _web.ReportProgress;
            set { _web.ReportProgress = value; }
        }
        public Action ReportProgessComplete
        {
            get => _web.ReportProgessComplete;
            set { _web.ReportProgessComplete = value; }
        }
        public bool DriverReadHeadless
        {
            get => _web.DriverReadHeadless;
            set { _web.DriverReadHeadless = value; }
        }

        public WebFetchResult Fetch()
        {
            var uploadNeeded = true;
            var result = new WebFetchResult();
            try
            {
                var now = DateTime.UtcNow;
                var days = Convert.ToInt32(Math.Round(now.Subtract(StartDate).TotalDays, 0));
                if (!UseRemoteDb || days < RemoteMinDayInterval)
                {
                    result = _web.Fetch();
                    return result;
                }
                uploadNeeded = false;
                // break dates
                var startDt = StartDate;
                var endinDt = EndingDate;
                var dates = DallasSearchProcess.GetBusinessDays(StartDate, EndingDate);
                dates.ForEach(d =>
                {

                    WebRequest.SearchDate = d.Date;
                    var begin = _dbsvc.Begin(WebRequest);
                    if (begin.RecordCount > 0 && begin.CompleteDate.HasValue)
                    {
                        var found = _dbsvc.Query(new QueryDbRequest { Id = begin.Id });
                        var people = ConvertFrom(found);
                        result.PeopleList.AddRange(people);
                    }
                    else
                    {
                        _web.StartDate = d;
                        _web.EndingDate = d;
                        var tmp = _web.Fetch();
                        if (tmp.PeopleList.Count > 0)
                        {
                            PostResult(tmp);
                            result.PeopleList.AddRange(tmp.PeopleList);
                        }
                        if (File.Exists(tmp.Result)) File.Delete(tmp.Result);
                    }
                });
                var nameService = new FileNameService(WebRequest, startDt, endinDt);
                var builder = new FileContentBuilder(
                    WebRequest.CountyId,
                    nameService.GetFileName(),
                    nameService.GetCourtTypeName(),
                    result.PeopleList);
                result.Result = builder.Generate();
                return result;
            }
            finally
            {
                if (uploadNeeded)
                {
                    PostResult(result);
                }
            }
        }

        private void PostResult(WebFetchResult result)
        {
            if (result.PeopleList.Count == 0) return;
            var people = result.PeopleList;
            var filingDates = people
                .Where(w => !string.IsNullOrEmpty(w.DateFiled))
                .Select(x => x.DateFiled)
                .Distinct()
                .ToList();
            foreach (var filingDate in filingDates)
            {
                if (!DateTime.TryParse(filingDate,
                    CultureInfo.CurrentCulture.DateTimeFormat,
                    DateTimeStyles.AssumeLocal,
                    out var dte)) continue;
                WebRequest.SearchDate = dte.Date;
                var begin = _dbsvc.Begin(WebRequest);
                var found = people.FindAll(x => x.DateFiled == filingDate);
                var payload = ConvertFrom(found);
                var request = new UploadDbRequest
                {
                    Id = begin.Id,
                    Contents = payload
                };
                if (_dbsvc.Upload(request)) _dbsvc.Complete(WebRequest);

            }
        }

        public string ReadFromFile(string result)
        {
            throw new NotImplementedException();
        }

        public string RemoveElement(string tableHtml, string tagName)
        {
            throw new NotImplementedException();
        }

        public string RemoveTag(string tableHtml, string tagName)
        {
            throw new NotImplementedException();
        }

        private static List<PersonAddress> ConvertFrom(List<QueryDbResponse> addresses)
        {
            var list = new List<PersonAddress>();
            if (addresses == null) return list;
            addresses.ForEach(a => list.Add(new PersonAddress
            {
                Name = a.Name,
                Zip = a.Zip,
                Address1 = a.Address1,
                Address2 = a.Address2,
                Address3 = a.Address3,
                CaseNumber = a.CaseNumber,
                DateFiled = a.DateFiled,
                Court = a.Court,
                CaseType = a.CaseType,
                CaseStyle = a.CaseStyle,
                Plantiff = a.Plaintiff,
            }));
            return list;
        }
        private static List<QueryDbResponse> ConvertFrom(List<PersonAddress> addresses)
        {
            var list = new List<QueryDbResponse>();
            if (addresses == null) return list;
            addresses.ForEach(a => list.Add(new QueryDbResponse
            {
                Name = a.Name,
                Zip = a.Zip,
                Address1 = a.Address1,
                Address2 = a.Address2,
                Address3 = a.Address3,
                CaseNumber = a.CaseNumber,
                DateFiled = a.DateFiled,
                Court = a.Court,
                CaseType = a.CaseType,
                CaseStyle = a.CaseStyle,
                Plaintiff = a.Plantiff,
            }));
            return list;
        }

        private static T GetValueOrDefault<T>(bool canParse, T current, T defaultValue)
        {
            if (!canParse) return defaultValue;
            return current ?? defaultValue;
        }

        private sealed class SettingLookupDto
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string DefaultValue { get; set; }
            public string GetValue()
            {
                if (string.IsNullOrEmpty(Value)) { return DefaultValue; }
                return Value;
            }
        }

        private static readonly SessionSettingPersistence UserDataReader =
            SessionPersistenceContainer
            .GetContainer
            .GetInstance<SessionSettingPersistence>();
        private readonly List<UserSettingChangeModel> sourceData =
            UserDataReader.GetList<UserSettingChangeModel>();

        private class FileNameService
        {
            private readonly int CountyId;
            private readonly int CaseTypeId;
            private readonly DateTime startDate;
            private readonly DateTime endDate;
            public FileNameService(
                FindDbRequest dbRequest,
                DateTime startingDate,
                DateTime endingDate)
            {
                CountyId = dbRequest.CountyId;
                CaseTypeId = dbRequest.CaseTypeId;
                startDate = startingDate;
                endDate = endingDate;
            }
            public string GetFileName()
            {
                var folder = ExcelDirectoyName();
                var county = GetCountyName();
                var fmt = $"{county}_{GetDateString(startDate)}_{GetDateString(endDate)}";
                var fullName = Path.Combine(folder, $"{fmt}.xlsx");
                var idx = 1;
                while (File.Exists(fullName))
                {
                    fullName = Path.Combine(folder, $"{fmt}_{idx:D4}.xlsx");
                    idx++;
                }
                return fullName;
            }

            public string GetCourtTypeName()
            {
                return CountyId switch
                {
                    1 => "Denton",
                    10 => "Tarrant",
                    20 => "Collin",
                    30 => "Harris",
                    40 => "Harris",
                    60 => "JUSTICE",
                    70 => "Travis",
                    80 => "Bexar",
                    90 => "Hidalgo",
                    100 => "El_Paso",
                    110 => "Fort_Bend",
                    120 => "Williamson",
                    130 => "Grayson",
                    _ => "",
                };
            }
            private string GetCountyName()
            {
                return CountyId switch
                {
                    1 => "Denton",
                    10 => "Tarrant",
                    20 => "Collin",
                    30 => "Harris",
                    40 => "Harris",
                    60 => "Dallas",
                    70 => "Travis",
                    80 => "Bexar",
                    90 => "Hidalgo",
                    100 => "El_Paso",
                    110 => "Fort_Bend",
                    120 => "Williamson",
                    130 => "Grayson",
                    _ => "Extract",
                };
            }

            private static string GetDateString(DateTime date)
            {
                const string fmt = "yyMMdd";
                return date.ToString(fmt, Culture);
            }

            private static string ExcelDirectoyName()
            {
                var appFolder =
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var xmlFolder = Path.Combine(appFolder, "data");
                if (!Directory.Exists(xmlFolder)) Directory.CreateDirectory(xmlFolder);
                return xmlFolder;
            }
            private static readonly CultureInfo Culture = CultureInfo.CurrentCulture;
        }

        private class FileContentBuilder
        {
            private readonly string excelFileName;
            private readonly string courtTypeName;
            private readonly int countyId;
            private readonly List<PersonAddress> People;
            public FileContentBuilder(
                int countyIndex,
                string fileName,
                string courtType,
                List<PersonAddress> people)
            {
                excelFileName = fileName;
                courtTypeName = courtType;
                countyId = countyIndex;
                People = people;
            }

            public string Generate()
            {
                var writer = new ExcelWriter();
                var content = writer.ConvertToPersonTable(
                    addressList: People,
                    worksheetName: "addresses",
                    websiteId: countyId);
                var courtlist = People.Select(p =>
                {
                    if (string.IsNullOrEmpty(p.Court)) return string.Empty;
                    var find = GetAddress(countyId, courtTypeName, p.Court);
                    if (string.IsNullOrEmpty(find)) return string.Empty;
                    return find;
                }).ToList();
                content.TransferColumn("County", "fname");
                content.TransferColumn("CourtAddress", "lname");
                content.PopulateColumn("CourtAddress", courtlist);
                using (var ms = new MemoryStream())
                {
                    content.SaveAs(ms);
                    var data = ms.ToArray();
                    File.WriteAllBytes(excelFileName, data);
                }
                return excelFileName;
            }

            private static string GetAddress(int courtId, string name, string court)
            {
                return courtId switch
                {
                    60 => DallasCourtLookupService.GetAddress(name, court),
                    _ => string.Empty
                };
            }
        }
    }
}
