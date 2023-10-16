using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Model
{
    public class ApplicationRequestModel
    {
        [Required(ErrorMessage = "{0} is a required field.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} is a required field.")]
        public Guid? Id { get; set; }
    }
}
