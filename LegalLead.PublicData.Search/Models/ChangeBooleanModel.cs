using LegalLead.PublicData.Search.Attr;
using LegalLead.PublicData.Search.Interfaces;

namespace LegalLead.PublicData.Search.Models
{
    internal class ChangeBooleanModel : ISettingChangeModel
    {
        [BooleanRequest(ErrorMessage = "Invalid value. Should be either 'true' or 'false'")]
        public string Text { get; set; }
    }
}
