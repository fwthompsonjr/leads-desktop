using Newtonsoft.Json;

namespace LegalLead.PublicData.Search.Classes
{
    public class HccConfigurationSetting
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }
    }
}
