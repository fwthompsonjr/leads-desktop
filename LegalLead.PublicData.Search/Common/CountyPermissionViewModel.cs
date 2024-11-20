namespace LegalLead.PublicData.Search.Common
{
    public class CountyPermissionViewModel
    {
        public int CountyId { get; set; } = 0;
        public string CountyName { get; set; }
        public bool IsEnabled { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string ButtonText { get; } = "Change";
    }
}
