using Newtonsoft.Json;
using System;
using System.Linq;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class HccConfigurationModel
    {
        public string Url { get; set; } = string.Empty;
        public string Monthly { get; set; } = string.Empty;
        public string Settings { get; set; } = string.Empty;

        [JsonProperty("remote")] public HccRemoteUriModel RemoteModel { get; set; } = new HccRemoteUriModel();

        [JsonProperty("db")] public DbUriModel DbModel { get; set; } = new DbUriModel();
        [JsonProperty("invoice")] public InvoiceUriModel InvoiceModel { get; set; } = new InvoiceUriModel();
        public static HccConfigurationModel GetModel()
        {
            if (model != null) return model;
            var tmp = JsonConvert.DeserializeObject<HccConfigurationModel>(configurationJs) ?? new HccConfigurationModel();
            model = tmp;
            return model;
        }
        private static HccConfigurationModel model = null;
        private static readonly string configurationJs = Properties.Resources.hcc_scripts;
    }
    public class HccRemoteUriModel
    {
        [JsonProperty("uri")] public string Url { get; set; }
        [JsonProperty("post")] public string PostUrl { get; set; }
        [JsonProperty("get")] public string FetchUrl { get; set; }
        [JsonProperty("count")] public string CountUrl { get; set; }
    }
    public class DbUriModel
    {
        [JsonProperty("uri")] public string Url { get; set; }
        [JsonProperty("remote-uri")] public string RemoteUrl { get; set; }

        [JsonProperty("debug-uri")] public string DebugUrl { get; set; }
        [JsonProperty("admin")] public string AdminUrl { get; set; }
        [JsonProperty("begin")] public string BeginUrl { get; set; }
        [JsonProperty("complete")] public string CompleteUrl { get; set; }
        [JsonProperty("query")] public string QueryUrl { get; set; }
        [JsonProperty("upload")] public string UploadUrl { get; set; }
        [JsonProperty("holiday")] public string HolidayUrl { get; set; }
        [JsonProperty("usage-append")] public string UsageAppendRecordUrl { get; set; } = string.Empty;
        [JsonProperty("usage-complete")] public string UsageCompleteRecordUrl { get; set; } = string.Empty;
        [JsonProperty("usage-get-limits")] public string UsageGetLimitsUrl { get; set; } = string.Empty;
        [JsonProperty("usage-get-history")] public string UsageGetHistoryUrl { get; set; } = string.Empty;
        [JsonProperty("usage-get-summary")] public string UsageGetSummaryUrl { get; set; } = string.Empty;
        [JsonProperty("usage-set-limit")] public string UsageSetLimitUrl { get; set; } = string.Empty;
        [JsonProperty("content-get")] public string ContentGetUrl { get; set; } = string.Empty;
        [JsonProperty("content-save")] public string ContentSaveUrl { get; set; } = string.Empty;
        [JsonProperty("process-offline")] public string BeginSearchUrl { get; set; }
        [JsonProperty("process-offline-status")] public string SearchStatusUrl { get; set; }
        [JsonProperty("get-offline-requests")] public string OfflineStatusUrl { get; set; }
        [JsonProperty("get-offline-download-status")] public string DownloadStatusUrl { get; set; }
        [JsonProperty("set-offline-download-complete")] public string DownloadIsCompletedUrl { get; set; }
        [JsonProperty("process-offline-set-context")] public string OfflineCountyTypeUrl { get; set; }
        [JsonProperty("get-offline-request-search-details")] public string SearchDetailsUrl { get; set; }
        [JsonProperty("register-account")] public string RegisterAccountUrl { get; set; }
        [JsonProperty("get-my-profile")] public string GetProfileUrl { get; set; }
        [JsonProperty("update-my-profile")] public string UpdateProfileUrl { get; set; }
        public string GetUri()
        {
            var collection = "remote,debug".Split(',');
            if (string.IsNullOrWhiteSpace(Url)) return Url;
            if (!collection.Contains(Url)) return Url;
            if (Url.Equals(collection[0])) return RemoteUrl;
            if (Url.Equals(collection[1])) return DebugUrl;
            return Url;
        }
    }

    public class InvoiceUriModel
    {
        [JsonProperty("uri")] public string Url { get; set; }
        [JsonProperty("fetch")] public string FetchUrl { get; set; }
        [JsonProperty("invoice-creation")] public string InvoiceGenerationUrl { get; set; }
        [JsonProperty("preview")] public string PreviewUrl { get; set; }
        [JsonProperty("status")] public string StatusUrl { get; set; }
    }
}
