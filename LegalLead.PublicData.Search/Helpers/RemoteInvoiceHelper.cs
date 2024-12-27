using LegalLead.PublicData.Search.Interfaces;
using System.Net.Http;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Helpers
{
    public class RemoteInvoiceHelper : IRemoteInvoiceHelper
    {
        private readonly IHttpService httpService;
        public RemoteInvoiceHelper(IHttpService http)
        {
            httpService = http;
        }

        public string CreateInvoice(string invoiceData)
        {
            var fallback = string.Empty;
            var payload = invoiceData.ToInstance<InvoiceHeaderModel>();
            if (payload == null) return fallback;
            var uri = GetAddress("invoice-creation");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri)) return fallback;
            var request = new
            {
                CustomerId = GetLeadId(),
                RequestType = "Invoice",
                InvoiceId = payload.Id,
            };
            using var client = GetClient(token);
            var response = httpService.PostAsJson<object, object>(client, uri, request);
            if (response == null) return fallback;
            return response.ToJsonString();
        }

        public string GetInvoicesByCustomerId()
        {
            var fallback = string.Empty;
            var uri = GetAddress("fetch");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri)) return fallback;
            var request = new
            {
                CustomerId = GetLeadId(),
                RequestType = "Customer",
                InvoiceId = ""
            };
            using var client = GetClient(token);
            var response = httpService.PostAsJson<object, object>(client, uri, request);
            if (response == null) return fallback;
            return response.ToJsonString();
        }

        public string PreviewInvoice(string invoiceData)
        {
            _ = CreateInvoice(invoiceData);
            var fallback = string.Empty;
            var payload = invoiceData.ToInstance<InvoiceHeaderModel>();
            if (payload == null) return fallback;
            var uri = GetAddress("preview");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri)) return fallback;
            var request = new
            {
                CustomerId = GetLeadId(),
                RequestType = "Invoice",
                InvoiceId = payload.Id,
            };
            using var client = GetClient(token);
            var response = httpService.PostAsJson<object, object>(client, uri, request);
            if (response == null) return fallback;
            if (response is not string html) return fallback;
            return html;
        }

        private static string GetLeadId()
        {
            return GetUser()?.User.Id ?? string.Empty;
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
            var obj = GetUser();
            return obj?.AuthenicationToken ?? string.Empty;
        }

        private static LeadUserSecurityBo GetUser()
        {
            var serialized = SessionUtil.Read();
            if (string.IsNullOrEmpty(serialized)) return default;
            return serialized.ToInstance<LeadUserSecurityBo>();
        }

        private static string GetAddress(string name)
        {
            var provider = AddressBuilder.InvoiceModel;
            var uri = provider.Url;
            return name switch
            {
                "fetch" => $"{uri}{provider.FetchUrl}",
                "invoice-creation" => $"{uri}{provider.InvoiceGenerationUrl}",
                "preview" => $"{uri}{provider.PreviewUrl}",
                "status" => $"{uri}{provider.StatusUrl}",
                _ => string.Empty
            };
        }
        private static readonly HccConfigurationModel AddressBuilder = HccConfigurationModel.GetModel();
    }
}
