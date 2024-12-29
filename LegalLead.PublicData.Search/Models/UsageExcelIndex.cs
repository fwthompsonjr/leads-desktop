namespace LegalLead.PublicData.Search.Models
{
    internal class UsageExcelIndex
    {
        public string Id { get; set; } = string.Empty;
        public string LeadUserId { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public string ExcelName { get; set; } = string.Empty;
        public string Status { get; internal set; }
    }
}
