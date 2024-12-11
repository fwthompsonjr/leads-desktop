using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Thompson.RecordSearch.Utility;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Util
{
    public class UiDbInteractive : IWebInteractive
    {
        #region Constructors

        public UiDbInteractive(
            IWebInteractive interactive,
            IRemoteDbHelper db,
            FindDbRequest request)
        {
            _web = interactive;
            _dbsvc = db;
            DataSearchParameters = request;
            var settings = MapDbSettings();
            UseRemoteDb = settings.UseRemoteDb;
            RemoteMinDayInterval = settings.RemoteMinDayInterval;
        }

        #endregion

        #region Private Fields

        private readonly bool UseRemoteDb;
        private readonly int RemoteMinDayInterval;
        private readonly FindDbRequest DataSearchParameters;
        private readonly IRemoteDbHelper _dbsvc;
        private readonly IWebInteractive _web;

        #endregion

        #region Public Fields

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
        public Action<int, int, int, string> ReportProgress
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

        #endregion

        #region Public Methods

        public WebFetchResult Fetch()
        {
            var uploadNeeded = true;
            var result = new WebFetchResult
            {
                PeopleList = new List<PersonAddress>(),
                WebsiteId = DataSearchParameters.CountyId,
                CaseList = string.Empty
            };
            try
            {
                var startDt = StartDate;
                var endingDt = EndingDate;
                ParameterHelper.AddOrUpdate(ParameterName.StartDate, startDt, Parameters);
                ParameterHelper.AddOrUpdate(ParameterName.EndDate, endingDt, Parameters);
                if (UseDataBaseInteration())
                {
                    uploadNeeded = false;
                    IterateDateRange(result);
                    GenerateExcelFile(result, startDt, endingDt);
                    return result;
                }

                result = _web.Fetch();
                result.WebsiteId = DataSearchParameters.CountyId;
                return result;
            }
            finally
            {
                if (uploadNeeded) PostResult(result);
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

        #endregion

        #region Private Methods

        private void GenerateExcelFile(
            WebFetchResult result,
            DateTime startDt,
            DateTime endingDt)
        {
            var nameService = new FileNameService(DataSearchParameters, startDt, endingDt);
            var builder = new FileContentBuilder(
                DataSearchParameters.CountyId,
                nameService.GetFileName(),
                nameService.GetCourtTypeName(),
                result.PeopleList);
            result.Result = builder.Generate();
            result.CaseList = JsonConvert.SerializeObject(result.PeopleList);
        }

        private void IterateDateRange(WebFetchResult result)
        {
            var dates = DallasSearchProcess.GetBusinessDays(StartDate, EndingDate);
            var currentDt = 1;
            var maximumDt = dates.Count;
            dates.ForEach(d =>
            {
                var notification = $"{d:d}";
                this.EchoProgess(0, maximumDt, currentDt++, message: $"Processing date: {d:d}", calcPercentage: false, dateNotification: notification);
                DataSearchParameters.SearchDate = d.Date;
                if (!ReadResultFromDataBase(result, DataSearchParameters))
                {
                    ReadResultFromWebSite(d, result);
                }
            });
            this.EchoProgess(0, 0, 0, dateNotification: "hide");
        }

        private bool ReadResultFromDataBase(WebFetchResult result, FindDbRequest request)
        {
            var begin = _dbsvc.Begin(request);
            if (begin == null) return false;
            if (begin.RecordCount <= 0 || !begin.CompleteDate.HasValue) return false;
            var found = _dbsvc.Query(new QueryDbRequest { Id = begin.Id });
            if (found == null || found.Count == 0) return false;
            var people = ConvertFrom(found);
            people.RemoveAll(IsEmpty);
            if (people.Count == 0) return false;
            result.PeopleList ??= new();
            result.PeopleList.AddRange(people);
            return true;
        }

        private void ReadResultFromWebSite(DateTime d, WebFetchResult result)
        {
            _web.StartDate = d;
            _web.EndingDate = d;
            ParameterHelper.AddOrUpdate(ParameterName.StartDate, d, _web.Parameters);
            ParameterHelper.AddOrUpdate(ParameterName.EndDate, d, _web.Parameters);
            var tmp = _web.Fetch();
            tmp.PeopleList?.RemoveAll(IsEmpty);
            if (tmp.PeopleList != null && tmp.PeopleList.Count > 0)
            {
                tmp.WebsiteId = DataSearchParameters.CountyId;
                PostResult(tmp, d);
                result.PeopleList.AddRange(tmp.PeopleList);
            }
            DeleteTempFiles(tmp.Result);
        }

        private DataSetting MapDbSettings()
        {
            var list = sourceData.FindAll(x => x.Category == "browser");
            var lookups = new[] {
                new SettingLookupDto {
                    Name = "Database Search:",
                    DefaultValue = "true"
                },
                new SettingLookupDto {
                    Name = "Database Minimun Persistence:",
                    DefaultValue = "5"
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
            var remoteDb = GetValueOrDefault(bUseDb, bUse, true);
            var remoteMinDayInterval = GetValueOrDefault(bMinDays, minInterval, 15);
            return new DataSetting
            {
                RemoteMinDayInterval = remoteMinDayInterval,
                UseRemoteDb = remoteDb
            };
        }

        private bool UseDataBaseInteration()
        {
            if (DataSearchParameters.CountyId == 1) return false;
            if (DataSearchParameters.CountyId == 30 && DataSearchParameters.CourtTypeName == "CRIMINAL") return false;

            if (!UseRemoteDb) return false;
            var now = DateTime.UtcNow;
            var days = Convert.ToInt32(Math.Round(now.Subtract(StartDate).TotalDays, 0));
            if (days <= RemoteMinDayInterval) return false;
            return true;
        }

        private void PostResult(WebFetchResult result, DateTime? searchDate = null)
        {
            try
            {
                if (result.PeopleList == null || result.PeopleList.Count == 0) return;
                var people = result.PeopleList;
                people.RemoveAll(IsEmpty);
                if (people.Count == 0) return;
                if (result.WebsiteId == (int)SourceType.BexarCounty)
                {
                    PostBexarDataResult(searchDate, people);
                    return;
                }
                PostDataResult(people);
            }
            catch (Exception)
            {
                // intented blank
                // future version can log exception to remote db
            }
        }

        private void PostDataResult(List<PersonAddress> people)
        {
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
                DataSearchParameters.SearchDate = dte.Date;
                var begin = _dbsvc.Begin(DataSearchParameters);
                var found = people.FindAll(x => x.DateFiled == filingDate);
                var payload = ConvertFrom(found);
                var request = new UploadDbRequest
                {
                    Id = begin.Id,
                    Contents = payload
                };
                var uploadResponse = _dbsvc.Upload(request);
                if (uploadResponse.Key) _dbsvc.Complete(DataSearchParameters);
            }
        }

        private void PostBexarDataResult(DateTime? searchDate, List<PersonAddress> people)
        {
            if (searchDate == null) return;
            DataSearchParameters.SearchDate = searchDate.Value.Date;
            var bxbegin = _dbsvc.Begin(DataSearchParameters);
            var bxpayload = ConvertFrom(people);
            var bxrequest = new UploadDbRequest
            {
                Id = bxbegin.Id,
                Contents = bxpayload
            };
            var uploadResponse = _dbsvc.Upload(bxrequest);
            if (uploadResponse.Key) _dbsvc.Complete(DataSearchParameters);
        }


        #endregion

        #region Private Static Methods

        private static void DeleteTempFiles(string tempFile)
        {
            if (string.IsNullOrEmpty(tempFile)) return;
            var parentDir = Path.GetDirectoryName(tempFile);
            var childFiles = new[] {
                Path.GetFileName(tempFile),
                $"{Path.GetFileNameWithoutExtension(tempFile)}.xlsx"
                };
            foreach (var shortName in childFiles)
            {
                var fullName = Path.Combine(parentDir, shortName);
                if (File.Exists(fullName)) File.Delete(fullName);
            }
        }
        private static List<PersonAddress> ConvertFrom(List<QueryDbResponse> addresses)
        {
            var list = new List<PersonAddress>();
            if (addresses == null) return list;
            addresses.ForEach(a => list.Add(new PersonAddress
            {
                Name = a.Name ?? string.Empty,
                Zip = a.Zip ?? string.Empty,
                Address1 = a.Address1 ?? string.Empty,
                Address2 = a.Address2 ?? string.Empty,
                Address3 = a.Address3 ?? string.Empty,
                CaseNumber = a.CaseNumber ?? string.Empty,
                DateFiled = a.DateFiled ?? string.Empty,
                Court = a.Court ?? string.Empty,
                CaseType = a.CaseType ?? string.Empty,
                CaseStyle = a.CaseStyle ?? string.Empty,
                Plantiff = a.Plaintiff ?? string.Empty,
            }));
            return list;
        }
        private static List<QueryDbResponse> ConvertFrom(List<PersonAddress> addresses)
        {
            var list = new List<QueryDbResponse>();
            if (addresses == null) return list;
            addresses.ForEach(a => list.Add(new QueryDbResponse
            {
                Name = a.Name ?? string.Empty,
                Zip = a.Zip ?? string.Empty,
                Address1 = a.Address1 ?? string.Empty,
                Address2 = a.Address2 ?? string.Empty,
                Address3 = a.Address3 ?? string.Empty,
                CaseNumber = a.CaseNumber ?? string.Empty,
                DateFiled = a.DateFiled ?? string.Empty,
                Court = a.Court ?? string.Empty,
                CaseType = a.CaseType ?? string.Empty,
                CaseStyle = a.CaseStyle ?? string.Empty,
                Plaintiff = a.Plantiff ?? string.Empty,
            }));
            return list;
        }
        private static bool IsEmpty(PersonAddress a)
        {
            if (string.IsNullOrWhiteSpace(a.CaseNumber)) return true;
            if (string.IsNullOrWhiteSpace(a.Zip)) return true;
            return false;
        }
        private static T GetValueOrDefault<T>(bool canParse, T current, T defaultValue)
        {
            if (!canParse) return defaultValue;
            return current ?? defaultValue;
        }

        #endregion

        #region Private Classes

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

        private class FileNameService
        {
            private readonly string CountyName;
            private readonly string CourtTypeName;
            private readonly DateTime startDate;
            private readonly DateTime endDate;
            public FileNameService(
                FindDbRequest dbRequest,
                DateTime startingDate,
                DateTime endingDate)
            {
                CountyName = dbRequest.CountyName;
                CourtTypeName = dbRequest.CourtTypeName;
                startDate = startingDate;
                endDate = endingDate;
            }
            public string GetFileName()
            {
                var folder = ExcelDirectoryName();
                var county = CountyName;
                if (!string.IsNullOrEmpty(CourtTypeName))
                {
                    county = string.Concat(county, "_", CourtTypeName);
                }
                var fmt = $"{county}_{GetDateString(startDate)}_{GetDateString(endDate)}";
                var fullName = Path.Combine(folder, $"{fmt}.xlsx");
                var idx = 1;
                while (File.Exists(fullName))
                {
                    fullName = Path.Combine(folder, $"{fmt}_{idx:D4}.xlsx");
                    idx++;
                }
                return fullName.ToUpper();
            }

            public string GetCourtTypeName()
            {
                return CourtTypeName;
            }

            private static string GetDateString(DateTime date)
            {
                const string fmt = "yyMMdd";
                return date.ToString(fmt, Culture);
            }

            private static string ExcelDirectoryName()
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
                    80 => BexarCourtLookupService.GetAddress(name, court),
                    100 => ElPasoCourtLookupService.GetAddress(name, court),
                    110 => FortBendCourtLookupService.GetAddress(name, court),
                    130 => GraysonCourtLookupService.GetAddress(name, court),
                    90 => HidalgoCourtLookupService.GetAddress(name, court),
                    70 => TravisCourtLookupService.GetAddress(name, court),
                    120 => WilliamsonCourtLookupService.GetAddress(name, court),
                    _ => AlternateCourtLookupService.GetAddress(courtId, court)
                };
            }
        }

        private class DataSetting
        {
            public bool UseRemoteDb { get; set; }
            public int RemoteMinDayInterval { get; set; }
        }

        private static class ParameterHelper
        {
            public static void AddOrUpdate(ParameterName field, DateTime value, WebNavigationParameter parameter)
            {
                if (parameter == null) return;
                const StringComparison oic = StringComparison.OrdinalIgnoreCase;
                var keyName = field == ParameterName.StartDate ?
                    CommonKeyIndexes.StartDate : CommonKeyIndexes.EndDate;
                var dtvalue = value.ToString("d", CultureInfo.CurrentCulture.DateTimeFormat);
                parameter.Keys ??= new List<WebNavigationKey>();
                var key = parameter.Keys.Find(x => x.Name.Equals(keyName, oic));
                if (key != null)
                {
                    key.Value = dtvalue;
                    return;
                }
                parameter.Keys.Add(new WebNavigationKey { Name = keyName, Value = dtvalue });
            }
        }

        #endregion

        #region Private Static Fields

        private static readonly SessionSettingPersistence UserDataReader =
            SessionPersistenceContainer
            .GetContainer
            .GetInstance<SessionSettingPersistence>();
        private readonly List<UserSettingChangeModel> sourceData =
            UserDataReader.GetList<UserSettingChangeModel>();

        #endregion

        #region Private Enum

        private enum ParameterName
        {
            StartDate,
            EndDate
        }

        #endregion
    }
}
