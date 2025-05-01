namespace LegalLead.PublicData.Search.Models
{
    public class AdminDbResponse
    {
        public string MethodName { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
