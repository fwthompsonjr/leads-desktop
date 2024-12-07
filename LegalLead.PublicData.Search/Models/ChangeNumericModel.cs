using LegalLead.PublicData.Search.Attr;
using LegalLead.PublicData.Search.Interfaces;

namespace LegalLead.PublicData.Search.Models
{
    public class ChangeNumericModel : ISettingChangeModel
    {
        [NumericRequest(
            ErrorMessage = "Invalid value. Please enter a valid number")]
        public string Text { get; set; }
    }
}