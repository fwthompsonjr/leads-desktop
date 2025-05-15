using Newtonsoft.Json;
using System.Collections.Generic;

namespace LegalLead.PublicData.Search.Models
{
    public class CommonJsScriptModel
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("json")]
        public List<string> JsonData { get; set; } = [];
    }
}
