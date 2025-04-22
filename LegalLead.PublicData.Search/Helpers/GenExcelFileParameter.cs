using System;

namespace LegalLead.PublicData.Search.Helpers
{
    public class GenExcelFileParameter
    {
        // int websiteId, string countyName, string courtType, string trackingIndex
        public int WebsiteId { get; set; }
        public string CountyName { get; set; }
        public string CourtType { get; set; }
        public string TrackingIndex { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}