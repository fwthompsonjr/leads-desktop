namespace Thompson.RecordSearch.Utility.Models
{
    public class LeadCountyTokenModel
    {
        public string LeadUserId { get; set; } = string.Empty;
        public string CountyName { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int? MonthlyLimit { get; set; } = 0;
    }
}
