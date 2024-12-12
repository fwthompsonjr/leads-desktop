using System;

namespace LegalLead.PublicData.Search.Models
{
    public class FindDbResponse
    {
        public string Id { get; set; } = string.Empty;
        public int CountyId { get; set; }
        public int RecordCount { get; set; }
        public DateTime SearchDate { get; set; }
        public int SearchTypeId { get; set; }
        public int CaseTypeId { get; set; }
        public int DistrictCourtId { get; set; }
        public int DistrictSearchTypeId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? CompleteDate { get; set; }
    }
}
