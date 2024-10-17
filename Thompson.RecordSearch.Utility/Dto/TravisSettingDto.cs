using Newtonsoft.Json;

namespace Thompson.RecordSearch.Utility.Dto
{

    public class TravisSettingDto
    {
        [JsonProperty("url-county")]
        public string CountyWebsite { get; set; } = "";
        [JsonProperty("url-district")]
        public string DistrictWebsite { get; set; } = "";
        [JsonProperty("url-justice")]
        public string JusticeWebsite { get; set; } = "";
        [JsonProperty("searchLink")]
        public string SearchLinkLocator { get; set; } = "";
    }
}
