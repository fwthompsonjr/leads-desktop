using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Helpers
{
    public class RemoteDbHelper : IRemoteDbHelper
    {
        public RemoteDbHelper(IHttpService http)
        {
            httpService = http;
        }
        private readonly IHttpService httpService;

        public AdminDbResponse Admin(AdminDbRequest findDb)
        {
            var uri = GetAddress("admin");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(token)) return null;
            using var client = GetClient(token);
            var response = httpService.PostAsJson<AdminDbRequest, AdminDbResponse>(client, uri, findDb);
            return response;
        }
        public FindDbResponse Begin(FindDbRequest findDb)
        {
            var uri = GetAddress("begin");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(token)) return null;
            using var client = GetClient(token);
            var response = httpService.PostAsJson<FindDbRequest, FindDbResponse>(client, uri, findDb);
            return response;
        }

        public FindDbResponse Complete(FindDbRequest findDb)
        {
            var uri = GetAddress("complete");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(token)) return null;
            using var client = GetClient(token);
            var response = httpService.PostAsJson<FindDbRequest, FindDbResponse>(client, uri, findDb);
            return response;
        }

        public List<QueryDbResponse> Query(QueryDbRequest queryDb)
        {
            var uri = GetAddress("query");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(token)) return null;
            using var client = GetClient(token);
            var response = httpService.PostAsJson<QueryDbRequest, List<QueryDbResponse>>(client, uri, queryDb);
            response.RemoveAll(x => string.IsNullOrWhiteSpace(x.DateFiled));
            return response;
        }

        public KeyValuePair<bool, string> Upload(UploadDbRequest uploadDb)
        {
            var uri = GetAddress("upload");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(token))
                return new KeyValuePair<bool, string>(false, "data validation error");
            var payload = new UploadDbRequest
            {
                Id = uploadDb.Id,
                Contents = new List<QueryDbResponse>()
            };
            var responses = new List<KeyValuePair<bool, string>>();
            var children = SplitList(uploadDb.Contents, 500);
            children.ForEach(x =>
            {
                payload.Contents.Clear();
                payload.Contents.AddRange(x);
                using var client = GetClient(token);
                var response = httpService.PostAsJson<UploadDbRequest, KeyValuePair<bool, string>>(client, uri, payload);
                responses.Add(response);
            });
            var hasError = responses.Exists(x => !x.Key);
            return hasError ?
                responses.Find(x => !x.Key)
                : new KeyValuePair<bool, string>(true, "");
        }

        public List<HolidayQueryResponse> Holidays()
        {
            if (holidayQueries.Count > 0) return holidayQueries;
            var fallback = new List<HolidayQueryResponse>();
            var uri = GetAddress("holiday");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(token)) return fallback;
            var payload = new { HolidayDate = string.Empty };
            using var client = GetClient(token);
            var response = httpService.PostAsJson<object, List<HolidayQueryResponse>>(client, uri, payload) ?? fallback;
            if (response.Count > 0) holidayQueries.AddRange(response);
            return response;
        }

        public AppendUsageRecordResponse AppendUsage(int countyId, DateTime startDate, DateTime endDate)
        {
            var fallback = new AppendUsageRecordResponse();
            var uri = GetAddress("usage-append");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri)) return fallback;
            var request = new AppendUsageRecordRequest
            {
                LeadUserId = GetLeadId(),
                CountyId = countyId,
                CountyName = GetCountyName(countyId),
                StartDate = startDate,
                EndDate = endDate,
                RecordCount = 0
            };
            using var client = GetClient(token);
            var response = httpService.PostAsJson<AppendUsageRecordRequest, AppendUsageRecordResponse>(client, uri, request)
                ?? fallback;
            return response;
        }

        public CompleteUsageRecordResponse CompleteUsage(string recordId, int recordCount, string excelName = "")
        {
            var fallback = new CompleteUsageRecordResponse();
            if (string.IsNullOrEmpty(recordId)) return fallback;
            var uri = GetAddress("usage-complete");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri)) return fallback;
            var request = new
            {
                UsageRecordId = recordId,
                RecordCount = recordCount,
                ExcelName = excelName,
            };
            using var client = GetClient(token);
            var response = httpService.PostAsJson<object, CompleteUsageRecordResponse>(client, uri, request)
                ?? fallback;
            return response;
        }

        public GetMonthlyLimitResponse GetLimits(int countyId, bool getAllCounties)
        {
            var fallback = new GetMonthlyLimitResponse();
            var uri = GetAddress("usage-get-limits");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri)) return fallback;
            var request = new GetMonthlyLimitRequest
            {
                LeadId = GetLeadId(),
                CountyId = countyId,
                GetAllCounties = getAllCounties
            };
            using var client = GetClient(token);
            var response = httpService.PostAsJson<GetMonthlyLimitRequest, GetMonthlyLimitResponse>(client, uri, request)
                ?? fallback;
            return response;
        }


        public GetUsageResponse GetSummary(DateTime searchDate, bool getAllCounties)
        {
            var fallback = new GetUsageResponse();
            var uri = GetAddress("usage-get-summary");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri)) return fallback;
            var request = new GetUsageRequest
            {
                LeadId = GetLeadId(),
                SearchDate = searchDate,
                GetAllCounties = getAllCounties
            };
            using var client = GetClient(token);
            var response = httpService.PostAsJson<GetUsageRequest, GetUsageResponse>(client, uri, request)
                ?? fallback;
            return response;
        }

        public GetUsageResponse GetHistory(DateTime searchDate, bool getAllCounties)
        {
            var fallback = new GetUsageResponse();
            var uri = GetAddress("usage-get-history");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri)) return fallback;
            var request = new GetUsageRequest
            {
                LeadId = GetLeadId(),
                SearchDate = searchDate,
                GetAllCounties = getAllCounties
            };
            using var client = GetClient(token);
            var response = httpService.PostAsJson<GetUsageRequest, GetUsageResponse>(client, uri, request)
                ?? fallback;
            return response;
        }

        public void PostFileDetail(SearchContext context)
        {
            var payload = new
            {
                context.Id,
                FileType = context.FileFormat,
                context.FileStatus,
                FileContent = context.Content,
            };
            var uri = GetAddress("content-save");
            var token = GetToken();
            using var client = GetClient(token);
            httpService.PostAsJson<object, KeyValuePair<bool, string>>(client, uri, payload);
        }

        public SearchContext GetFileDetail(SearchContext context)
        {
            var payload = new
            {
                context.Id,
                FileType = context.FileFormat,
                context.FileStatus,
                FileContent = context.Content,
            };
            var uri = GetAddress("content-get");
            var token = GetToken();
            using var client = GetClient(token);
            var response = httpService.PostAsJson<object, SearchContext>(client, uri, payload)
             ?? context;
            return response;
        }

        public ProcessOfflineResponse BeginSearch(ProcessOfflineRequest request)
        {
            var uri = GetAddress("process-offline");
            var token = GetToken();
            using var client = GetClient(token);
            var response = httpService.PostAsJson<ProcessOfflineRequest, ProcessOfflineResponse>(client, uri, request);
            return response.ReplacePipe() ?? new();
        }
        public ProcessOfflineResponse GetSearchStatus(ProcessOfflineResponse request)
        {
            var uri = GetAddress("process-offline-status");
            var token = GetToken();
            using var client = GetClient(token);
            var response = httpService.PostAsJson<ProcessOfflineResponse, ProcessOfflineResponse>(client, uri, request);
            return response.ReplacePipe() ?? new();
        }

        public List<OfflineStatusResponse> GetOfflineRequests(OfflineStatusRequest request)
        {
            var uri = GetAddress("process-offline-status");
            var token = GetToken();
            using var client = GetClient(token);
            var response = httpService.PostAsJson<OfflineStatusRequest, List<OfflineStatusResponse>>(client, uri, request);
            if (response == null || response.Count == 0) return [];
            response.ForEach(r =>
            {
                r.OfflineRequestId = r.OfflineId;
                r.ReplacePipe();
            });
            return response;
        }

        public LeadUserModel RegisterAccount(RegisterAccountModel model)
        {
            var uri = GetAddress("register-account");
            var token = GetToken();
            var payload = new
            {
                userName = model.UserName,
                password = model.Password,
                email = model.Email,
            };
            using var client = GetClient(token);
            var response = httpService.PostAsJson<object, LeadUserModel>(client, uri, payload);
            return response ?? new();
        }

        private static List<List<T>> SplitList<T>(List<T> me, int size = 50)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < me.Count; i += size)
                list.Add(me.GetRange(i, Math.Min(size, me.Count - i)));
            return list;
        }

        private static HttpClient GetClient(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("LEAD_IDENTITY", token);
            return client;
        }
        private static string GetCountyName(int countyId)
        {
            return countyId switch
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
                100 => "ElPaso",
                110 => "FortBend",
                120 => "Williamson",
                130 => "Grayson",
                _ => "Unmapped"
            };
        }
        private static LeadUserSecurityBo GetUser()
        {
            var serialized = SessionUtil.Read();
            if (string.IsNullOrEmpty(serialized)) return default;
            return serialized.ToInstance<LeadUserSecurityBo>();
        }

        private static string GetLeadId()
        {
            return GetUser()?.User.Id ?? string.Empty;
        }

        private static string GetToken()
        {
            var serialized = SessionUtil.Read();
            if (string.IsNullOrEmpty(serialized)) return default;
            var obj = GetUser();
            return obj?.AuthenicationToken ?? string.Empty;
        }
        private static string GetAddress(string name)
        {
            var provider = AddressBuilder.DbModel;
#if DEBUG
            var uri = provider.GetUri();
#else
            var uri = provider.RemoteUrl;
#endif
            return name switch
            {
                "admin" => $"{uri}{provider.AdminUrl}",
                "begin" => $"{uri}{provider.BeginUrl}",
                "complete" => $"{uri}{provider.CompleteUrl}",
                "query" => $"{uri}{provider.QueryUrl}",
                "upload" => $"{uri}{provider.UploadUrl}",
                "holiday" => $"{uri}{provider.HolidayUrl}",
                "usage-append" => $"{uri}{provider.UsageAppendRecordUrl}",
                "usage-complete" => $"{uri}{provider.UsageCompleteRecordUrl}",
                "usage-get-limits" => $"{uri}{provider.UsageGetLimitsUrl}",
                "usage-get-history" => $"{uri}{provider.UsageGetHistoryUrl}",
                "usage-get-summary" => $"{uri}{provider.UsageGetSummaryUrl}",
                "usage-set-limit" => $"{uri}{provider.UsageSetLimitUrl}",
                "content-get" => $"{uri}{provider.ContentGetUrl}",
                "content-save" => $"{uri}{provider.ContentSaveUrl}",
                "process-offline" => $"{uri}{provider.BeginSearchUrl}",
                "process-offline-status" => $"{uri}{provider.SearchStatusUrl}",
                "get-offline-requests" => $"{uri}{provider.OfflineStatusUrl}",
                "register-account" => $"{uri}{provider.RegisterAccountUrl}",
                _ => string.Empty
            };
        }
        private static readonly HccConfigurationModel AddressBuilder = HccConfigurationModel.GetModel();
        private static readonly List<HolidayQueryResponse> holidayQueries = new();
    }
}