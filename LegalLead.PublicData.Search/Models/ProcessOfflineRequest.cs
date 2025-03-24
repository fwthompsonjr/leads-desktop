namespace LegalLead.PublicData.Search.Models
{
    public class ProcessOfflineRequest
    {
        public string Cookies { get; set; } = string.Empty;
        public string Workload { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
