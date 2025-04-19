using System;

namespace LegalLead.PublicData.Search.Models
{
    public class OfflineStatusResponse : ProcessOfflineResponse
    {
        public string LeadUserId { get; set; } = string.Empty;
        public string OfflineId { get; set; } = string.Empty;
        public string CountyName { get; set; } = string.Empty;
        public DateTime? SearchStartDate { get; set; }
        public DateTime? SearchEndDate { get; set; }
        public decimal? PercentComplete { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int RetryCount { get; set; }
    }
}
