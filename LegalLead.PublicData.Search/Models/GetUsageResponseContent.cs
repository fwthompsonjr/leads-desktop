using System;

namespace LegalLead.PublicData.Search.Models
{
    public class GetUsageResponseContent
    {
        public string UserName { get; set; } = string.Empty;
        public int SearchYear { get; set; }
        public int SearchMonth { get; set; }
        public int MonthlyLimit { get; set; }
        public string Id { get; set; } = string.Empty;
        public string LeadUserId { get; set; } = string.Empty;
        public int CountyId { get; set; }
        public string CountyName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DateRange { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public DateTime? CompleteDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}