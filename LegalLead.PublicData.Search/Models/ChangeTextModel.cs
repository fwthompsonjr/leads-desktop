using LegalLead.PublicData.Search.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace LegalLead.PublicData.Search.Models
{
    public class ChangeTextModel : ISettingChangeModel
    {
        [Required]
        [StringLength(250)]
        public string Text { get; set; }
    }
}
