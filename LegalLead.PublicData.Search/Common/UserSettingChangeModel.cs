namespace LegalLead.PublicData.Search.Common
{
    public class UserSettingChangeModel : UserSettingChangeViewModel
    {
        public int Index { get; set; }
        public bool IsSecured { get; set; }
        public string DataType { get; set; } = "Text";
    }
}
