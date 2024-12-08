using System;

namespace LegalLead.PublicData.Search.Models
{
    public class FindDbRequest
    {
        public int CountyId { get; set; }
        public DateTime SearchDate { get; set; }
        public int SearchTypeId { get; set; }
        public int CaseTypeId { get; set; }
        public int DistrictCourtId { get; set; }
        public int DistrictSearchTypeId { get; set; }
    }
}
