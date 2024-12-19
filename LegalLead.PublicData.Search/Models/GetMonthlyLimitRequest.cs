namespace LegalLead.PublicData.Search.Models
{
    public class GetMonthlyLimitRequest
    {
        public string LeadId { get; set; } = string.Empty;
        public int CountyId { get; set; }
        public bool GetAllCounties { get; set; }
    }
}
