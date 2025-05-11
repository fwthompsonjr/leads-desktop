using Newtonsoft.Json;
using System.Collections.Generic;

namespace LegalLead.PublicData.Search.Models
{
    public class TarrantConfigurationModel
    {
        [JsonProperty("basePage")]
        public string BasePage { get; set; } = string.Empty;

        [JsonProperty("locations")]
        public List<string> Locations { get; set; } = [];

        [JsonProperty("links")]
        public List<string> Links { get; set; } = [];
    }
}
