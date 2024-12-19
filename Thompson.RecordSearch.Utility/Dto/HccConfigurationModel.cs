using Newtonsoft.Json;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class HccConfigurationModel
    {
        public string Url { get; set; } = string.Empty;
        public string Monthly { get; set; } = string.Empty;
        public string Settings { get; set; } = string.Empty;

        [JsonProperty("remote")] public HccRemoteUriModel RemoteModel { get; set; } = new HccRemoteUriModel();

        [JsonProperty("db")] public DbUriModel DbModel { get; set; } = new DbUriModel();

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
    }
}
