using Newtonsoft.Json;

namespace LegalLead.PublicData.Search.Classes
{
    public class HccConfigurationProcess
    {
        [JsonProperty("loader")]
        public bool Loader { get; set; }
    }
}
