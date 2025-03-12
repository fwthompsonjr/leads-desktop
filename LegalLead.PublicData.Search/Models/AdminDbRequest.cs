namespace LegalLead.PublicData.Search.Models
{
    public class AdminDbRequest
    {
        public string MethodName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }
}