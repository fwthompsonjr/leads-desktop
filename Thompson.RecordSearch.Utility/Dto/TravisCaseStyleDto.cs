using Newtonsoft.Json;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class TravisCaseStyleDto
    {
        [JsonProperty("casestyle")]
        public string CaseStyle { get; set; }
        [JsonProperty("plaintiff")]
        public string Plaintiff { get; set; }
        [JsonProperty("defendant")]
        public string PartyName { get; set; }
        [JsonProperty("addr")]
        public string Address { get; set; } = string.Empty;
        public string Court { get; set; }
        public string CaseNumber { get; set; }
    }
}
