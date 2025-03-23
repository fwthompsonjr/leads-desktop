using System;

namespace LegalLead.PublicData.Search.Classes
{
    class GetSearchResponse
    {
        public string Id { get; set; }

        public string LeadUserId { get; set; }

        public int? CountyId { get; set; }

        public string CountyName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string DateRange { get; set; }

        public int? RecordCount { get; set; }

        public DateTime? CompleteDate { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
