namespace LegalLead.PublicData.Search.Models
{
    public class SetMonthlyLimitRequest
    {
        public string LeadId { get; set; } = string.Empty;
        public int CountyId { get; set; }
        public int MonthLimit { get; set; }
    }
}
