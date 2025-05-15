using Newtonsoft.Json;
using System.Collections.Generic;

namespace LegalLead.PublicData.Search.Models
{
    public class TarrantConfigurationBo
    {
        [JsonProperty("configuration")]
        public TarrantConfigurationModel Configuration { get; set; }

        [JsonProperty("scripts")]
        public List<CommonJsScriptModel> Scripts { get; set; }
    }
}
