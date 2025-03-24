using System.ComponentModel.DataAnnotations;

namespace LegalLead.PublicData.Search.Models
{
    public class RegisterAccountModel
    {
        private const string Pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,255}$";

        [Required]
        [MinLength(8, ErrorMessage = "{0} must have a minimum length of {1} characters")]
        [MaxLength(50, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "{0} must have a minimum length of {1} characters")]
        [MaxLength(255, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        [RegularExpression(Pattern, ErrorMessage = "Please enter a valid password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(255, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid e-mail address.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
