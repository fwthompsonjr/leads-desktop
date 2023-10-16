using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Model
{
    public class RegisterAccountModel
    {
        private const string Pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,32}$";

        [Required]
        [MinLength(8, ErrorMessage = "{0} must have a minimum length of 8 characters")]
        [MaxLength(50, ErrorMessage = "{0} must have a maximum length of 50 characters")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(Pattern, ErrorMessage = "Please enter a valid password")]
        public string Password { get; set; } = string.Empty;
    }
}
