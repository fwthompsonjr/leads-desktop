using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Helpers
{
    public class UserManagerGetInvoices : BaseUserManager, IUserManager
    {
        protected override string MethodName => "GetInvoice";

        public void BindGrid(DataGridView gridView, AdminDbResponse response)
        {
            try
            {
                if (gridView.DataSource != null)
                {
                    gridView.DataSource = null;
                    gridView.Columns?.Clear();
                }
                if (!response.IsSuccess)
                {
                    BindResponse(gridView, response);
                    return;
                }
                var rows = response.Message.ToInstance<List<Classes.GetInvoiceResponse>>() ?? [];
                gridView.DataSource = rows;
                // incomplete
                var incomplete = rows.FindAll(x => x.CompleteDate == null).ToJsonString();
                CheckStatusForIncompleteInvoice(incomplete);
                gridView.Columns["Id"].Visible = false;
                gridView.Columns["LeadUserId"].Visible = false;
                gridView.Columns["RequestId"].Visible = false;
                gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                gridView.Columns["InvoiceNbr"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                gridView.Columns["InvoiceNbr"].Width = 250; // Set a fixed width for the first column
                gridView.Columns["InvoiceUri"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                for (var col = 0; col < gridView.Columns.Count; col++)
                {
                    gridView.Columns[col].ReadOnly = true;
                }

            }
            finally
            {
                gridView.Refresh();
            }
        }

        private static void CheckStatusForIncompleteInvoice(string incomplete)
        {
            var rows = incomplete.ToInstance<List<Classes.GetInvoiceResponse>>() ?? [];
            if (rows.Count == 0) return;
            var uri = GetAddress("status");
            var token = GetToken();
            if (string.IsNullOrEmpty(uri)) return;
            _ = Task.Run(() =>
            {
                rows.ForEach(row =>
                {
                    var request = new
                    {
                        CustomerId = row.LeadUserId,
                        RequestType = "Invoice",
                        InvoiceId = row.Id,
                    };
                    using var client = GetClient(token);
                    httpService.PostAsJson<object, object>(client, uri, request);
                });
            });
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
                "get-billing-mode" => $"{uri}{provider.GetBillingCodeUrl}",
                "set-billing-mode" => $"{uri}{provider.SetBillingCodeUrl}",
                _ => string.Empty
            };
        }
        private static LeadUserSecurityBo GetUser()
        {
            var serialized = SessionUtil.Read();
            if (string.IsNullOrEmpty(serialized)) return default;
            return serialized.ToInstance<LeadUserSecurityBo>();
        }
        private static readonly IHttpService httpService =
            AuthenicationContainer.GetContainer.GetInstance<IHttpService>();
        private static readonly HccConfigurationModel AddressBuilder = HccConfigurationModel.GetModel();
    }
}