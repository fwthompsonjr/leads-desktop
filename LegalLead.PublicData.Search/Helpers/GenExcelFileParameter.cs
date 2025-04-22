using System;
using Thompson.RecordSearch.Utility.Models;

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
        public int RecordCount { get; set; }
        public string FileName { get; set; }


        public string GetShortName()
        {
            if (string.IsNullOrWhiteSpace(FileName)) return string.Empty;
            var fileName = System.IO.Path.GetFileNameWithoutExtension(FileName);
            var extn = System.IO.Path.GetExtension(FileName);
            if (string.IsNullOrWhiteSpace(fileName)) return FileName;
            if (string.IsNullOrWhiteSpace(extn)) extn = string.Empty;
            return string.Concat(fileName, extn);
        }
    }
}