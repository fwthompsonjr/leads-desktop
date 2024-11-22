using Newtonsoft.Json;
using System;

namespace Thompson.RecordSearch.Utility.Models
{
    public class UsageHistoryModel
    {
        [JsonProperty("countyName")]
        public string CountyName { get; set; }

        [JsonProperty("monthlyUsage")]
        public int MonthlyUsage { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }
        public string DateRange { get; set; }

        [JsonProperty("incidentMonth")]
        public int IncidentMonth { get; set; }

        [JsonProperty("incidentYear")]
        public int IncidentYear { get; set; }
    }
}
