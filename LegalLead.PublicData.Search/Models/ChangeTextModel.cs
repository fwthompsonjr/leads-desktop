using LegalLead.PublicData.Search.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace LegalLead.PublicData.Search.Models
{
    public class ChangeTextModel : ISettingChangeModel
    {
        [Required(ErrorMessage = "Invalid value. Field can not be empty")]
        [StringLength(250,
            ErrorMessage = "Invalid value. Field must be 255 characters or less")]
        public string Text { get; set; }
    }
}
