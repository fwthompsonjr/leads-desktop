using Newtonsoft.Json;

namespace LegalLead.PublicData.Search.Models
{
    public class TarrantCaseItemDto
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("caseNumber")]
        public string CaseNumber { get; set; }

        [JsonProperty("caseStyle")]
        public string CaseStyle { get; set; }

        [JsonProperty("caseType")]
        public string CaseType { get; set; }

        [JsonProperty("court")]
        public string Court { get; set; }

        [JsonProperty("dateFiled")]
        public string DateFiled { get; set; }

        [JsonProperty("charges")]
        public string Charges { get; set; }

        [JsonProperty("citationNumber")]
        public string CitationNumber { get; set; }
    }
}
