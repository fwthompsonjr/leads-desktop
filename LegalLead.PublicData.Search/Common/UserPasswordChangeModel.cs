using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LegalLead.PublicData.Search.Common
{
    internal class UserPasswordChangeModel
    {
        private const string Pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,255}$";

        [Required(ErrorMessage = "User Name is required.")]
        [JsonPropertyOrder(0)]
        [MinLength(8, ErrorMessage = "User Name must have a minimum length of 8 characters")]
        [MaxLength(255, ErrorMessage = "User Name must have a maximum length of 255 characters")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Current Password is required.")]
        [JsonPropertyOrder(1)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New Password is required.")]
        [JsonPropertyOrder(2)]
        [MinLength(8, ErrorMessage = "New Password must have a minimum length of 8 characters")]
        [MaxLength(255, ErrorMessage = "New Password must have a maximum length of 255 characters")]
        [RegularExpression(Pattern, ErrorMessage = "New Password must contain at least 2 upper case characters, 2 lower case characters, and 1 special character")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [JsonPropertyOrder(3)]
        [MinLength(8, ErrorMessage = "Confirm Password must have a minimum length of 8 characters")]
        [MaxLength(255, ErrorMessage = "Confirm Password must have a maximum length of 255 characters")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Confirm Password is not matched to New Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string this[int index]
        {
            get {
                var response = index switch
                {
                    0 => UserName,
                    1 => OldPassword,
                    2 => NewPassword,
                    3 => ConfirmPassword,
                    _ => string.Empty,
                };
                return response;
            }
            set {
                switch (index)
                {
                    case 0: UserName = value; break;
                    case 1: OldPassword = value; break;
                    case 2: NewPassword = value; break;
                    case 3: ConfirmPassword = value; break;
                }
            }
        }
    }
}
