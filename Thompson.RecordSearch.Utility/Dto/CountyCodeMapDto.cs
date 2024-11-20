using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class CountyCodeMapDto
    {
        public string Web { get; set; } = string.Empty;
        public CountyCodeLandingDto Landings { get; set; } = new CountyCodeLandingDto();

        [JsonProperty("apilandings")]
        public CountyCodeLandingDto ApiLandings { get; set; } = new CountyCodeLandingDto();
        public IEnumerable<CountyCodeDto> Counties { get; set; } = Enumerable.Empty<CountyCodeDto>();
    }

    public class CountyCodeLandingDto
    {
        [JsonProperty("login")] public string Login { get; set; } = string.Empty;
        [JsonProperty("county")] public string County { get; set; } = string.Empty;
        [JsonProperty("change")] public string Change { get; set; } = string.Empty;
        [JsonProperty("indexes")] public string Indexes { get; set; } = string.Empty;
        [JsonProperty("register")] public string Register { get; set; } = string.Empty;
        [JsonProperty("usage-set-limit")] public string UsageCreate { get; set; } = string.Empty;
        [JsonProperty("usage-add-record")] public string UsageIncrement { get; set; } = string.Empty;
        [JsonProperty("usage-list")] public string UsageList { get; set; } = string.Empty;
    }
}
