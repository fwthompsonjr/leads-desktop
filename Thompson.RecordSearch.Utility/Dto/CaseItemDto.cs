using Newtonsoft.Json;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class CaseItemDto
    {
        [JsonProperty("uri")]
        public string Href { get; set; }
        [JsonProperty("caseNumber")]
        public string CaseNumber { get; set; }
        [JsonProperty("filedate")]
        public string FileDate { get; set; }
        [JsonProperty("casetype")]
        public string CaseType { get; set; }
        [JsonProperty("casestatus")]
        public string CaseStatus { get; set; }
        [JsonProperty("location")]
        public string Court { get; set; }
        [JsonProperty("partyname")]
        public string PartyName { get; set; }
        public string Plaintiff { get; set; }
        [JsonProperty("caseStyle")]
        public string CaseStyle { get; set; } = string.Empty;
        [JsonProperty("courtDate")]
        public string CourtDate { get; set; } = string.Empty;
        [JsonProperty("hearingType")]
        public string HearingType { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
