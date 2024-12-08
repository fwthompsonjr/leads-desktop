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
            return response;
        }

        public bool Upload(UploadDbRequest uploadDb)
        {
            var uri = GetAddress("upload");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(token)) return false;
            var payload = new UploadDbRequest
            {
                Id = uploadDb.Id,
                Contents = new List<QueryDbResponse>()
            };
            var responses = new List<bool>();
            var children = SplitList(uploadDb.Contents, 500);
            children.ForEach(x =>
            {
                payload.Contents.Clear();
                payload.Contents.AddRange(x);
                using var client = GetClient(token);
                var response = httpService.PostAsJson<UploadDbRequest, bool>(client, uri, payload);
                responses.Add(response);
            });
            var hasError = responses.Exists(x => !x);
            return !hasError;
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

        private static string GetToken()
        {
            var serialized = SessionUtil.Read();
            if (string.IsNullOrEmpty(serialized)) return default;
            var obj = serialized.ToInstance<LeadUserSecurityBo>();
            return obj?.AuthenicationToken ?? string.Empty;
        }
        private static string GetAddress(string name)
        {
            var provider = AddressBuilder.DbModel;
            var uri = provider.Url;
            return name switch
            {
                "begin" => $"{uri}{provider.BeginUrl}",
                "complete" => $"{uri}{provider.CompleteUrl}",
                "query" => $"{uri}{provider.QueryUrl}",
                "upload" => $"{uri}{provider.UploadUrl}",
                _ => string.Empty
            };
        }
        private static readonly HccConfigurationModel AddressBuilder = HccConfigurationModel.GetModel();
    }
}