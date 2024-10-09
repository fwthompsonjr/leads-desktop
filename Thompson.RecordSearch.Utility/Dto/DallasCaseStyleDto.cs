using Newtonsoft.Json;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class DallasCaseStyleDto
    {
        [JsonProperty("casestyle")]
        public string CaseStyle { get; set; }
        [JsonProperty("plaintiff")]
        public string Plaintiff { get; set; }
        [JsonProperty("addr")]
        public string Address { get; set; } = string.Empty;
    }
}