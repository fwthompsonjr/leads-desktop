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


        public string GetInvoiceStatus(string invoiceData)
        {
            var fallback = string.Empty;
            var payload = invoiceData.ToInstance<InvoiceHeaderModel>();
            if (payload == null) return fallback;
            var uri = GetAddress("status");
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


        public string GetInvoicesByTrackingId(string customerId, string trackingId)
        {
            var fallback = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(customerId)) return fallback;
                if (string.IsNullOrEmpty(trackingId)) return fallback;
                var uri = GetAddress("get-by-tracking-id");
                var token = GetToken();
                if (string.IsNullOrEmpty(uri)) return fallback;
                var request = new
                {
                    CustomerId = customerId,
                    RequestType = "Customer",
                    InvoiceId = trackingId
                };
                using var client = GetClient(token);
                var response = httpService.PostAsJson<object, object>(client, uri, request);
                if (response == null) return fallback;
                return response.ToJsonString();
            }
            catch (System.Exception)
            {
                return fallback;
            }
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
            var response = httpService.GetFromJsonPost<object>(client, uri, request);
            if (response == null) return fallback;
            if (response is not string html) return fallback;
            return html;
        }

        public void UpdateExcelStatus(string json)
        {

        }

        public string GetBillingCode()
        {
            const string code = "TEST";
            try
            {
                var request = new { RequestType = "Customer" };
                var uri = GetAddress("get-billing-mode");
                var token = GetToken();
                using var client = GetClient(token);
                var response = httpService.PostAsJson<object, BillingCodeResponse>(client, uri, request);
                if (response == null || string.IsNullOrEmpty(response.BillingMode)) return code;
                return response.BillingMode;
            }
            catch (System.Exception)
            {
                return code;
            }
        }

        public string SetBillingCode(string leadId, string billingMode)
        {
            const string code = "TEST";
            try
            {
                var request = new { Id = leadId, BillingCode = billingMode };
                var uri = GetAddress("set-billing-mode");
                var token = GetToken();
                using var client = GetClient(token);
                var response = httpService.PostAsJson<object, BillingCodeResponse>(client, uri, request);
                if (response == null || string.IsNullOrEmpty(response.BillingMode)) return code;
                return response.BillingMode;
            }
            catch (System.Exception)
            {
                return code;
            }
        }
        private static string GetLeadId()
        {
            return GetUser()?.User.Id ?? string.Empty;
        }

        private static HttpClient GetClient(string token)
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };
            var client = new HttpClient(handler);
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
            var uri = $"{GetBaseUri()}db-invoice/";
            return name switch
            {
                "fetch" => $"{uri}{provider.FetchUrl}",
                "invoice-creation" => $"{uri}{provider.InvoiceGenerationUrl}",
                "preview" => $"{uri}{provider.PreviewUrl}",
                "status" => $"{uri}{provider.StatusUrl}",
                "get-billing-mode" => $"{uri}{provider.GetBillingCodeUrl}",
                "set-billing-mode" => $"{uri}{provider.SetBillingCodeUrl}",
                "get-by-tracking-id" => $"{uri}{provider.FindByTrackingIdUrl}",
                _ => string.Empty
            };
        }
        private class BillingCodeResponse
        {
            public string UserName { get; set; } = string.Empty;
            public string RequestedCode { get; set; } = string.Empty;
            public string BillingMode { get; set; }
        }
        private static string GetBaseUri()
        {

            var provider = AddressBuilder.DbModel;
#if DEBUG
            var uri = provider.GetUri();
#else
            var uri = provider.RemoteUrl;
#endif
            return uri;
        }
        private static readonly HccConfigurationModel AddressBuilder = HccConfigurationModel.GetModel();
    }
}
