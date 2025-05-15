using Newtonsoft.Json;

namespace LegalLead.PublicData.Search.Models
{
    public class HarrisCivilConfigurationModel
    {
        [JsonProperty("basePage")]
        public string BasePage { get; set; } = string.Empty;
    }
}