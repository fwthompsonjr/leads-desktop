using System;

namespace LegalLead.PublicData.Search.Classes
{
    public class GetCountyResponse
    {
        public string Id { get; set; }
        public int? RwId { get; set; }
        public int? CountyId { get; set; }
        public string LeadUserId { get; set; }
        public string CountyName { get; set; }
        public string Phrase { get; set; }
        public string Vector { get; set; }
        public string Token { get; set; }
        public int? MonthlyUsage { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
