using LegalLead.PublicData.Search.Attr;
using LegalLead.PublicData.Search.Interfaces;

namespace LegalLead.PublicData.Search.Models
{
    internal class ChangeDateModel : ISettingChangeModel
    {
        [DateTimeRequest(
            ErrorMessage = "Invalid value. Please enter a valid date")]
        public string Text { get; set; }
    }
}
