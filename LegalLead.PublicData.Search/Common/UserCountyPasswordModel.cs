using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LegalLead.PublicData.Search.Common
{
    public class UserCountyPasswordModel
    {
        [Required(ErrorMessage = "User Name is required.")]
        [JsonProperty("userName")]
        [JsonPropertyOrder(0)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [JsonPropertyOrder(1)]
        [JsonProperty("password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "County Name is required.")]
        [JsonPropertyOrder(2)]
        [JsonProperty("countyName")]
        public string CountyName { get; set; } = string.Empty;
    }
}