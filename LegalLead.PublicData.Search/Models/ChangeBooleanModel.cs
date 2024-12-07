using LegalLead.PublicData.Search.Attr;
using LegalLead.PublicData.Search.Interfaces;

namespace LegalLead.PublicData.Search.Models
{
    internal class ChangeBooleanModel : ISettingChangeModel
    {
        [BooleanRequest]
        public string Text { get; set; }
    }
}
