using System;

namespace LegalLead.PublicData.Search.Models
{
    public class GetUsageRequest
    {
        public string LeadId { get; set; } = string.Empty;
        public DateTime SearchDate { get; set; }
        public bool GetAllCounties { get; set; }
    }
}
