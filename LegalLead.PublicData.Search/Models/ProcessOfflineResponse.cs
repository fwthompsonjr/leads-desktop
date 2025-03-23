using System.Collections.Generic;

namespace LegalLead.PublicData.Search.Models
{
    public class ProcessOfflineResponse
    {
        public string RequestId { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string OfflineRequestId { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<string> Messages { get; set; } = [];
        public int RecordCount { get; set; }
        public int TotalProcessed { get; set; }
    }
}
