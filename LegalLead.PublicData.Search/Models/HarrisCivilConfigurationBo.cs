using Newtonsoft.Json;
using System.Collections.Generic;

namespace LegalLead.PublicData.Search.Models
{
    public class HarrisCivilConfigurationBo
    {
        [JsonProperty("configuration")]
        public HarrisCivilConfigurationModel Configuration { get; set; }

        [JsonProperty("scripts")]
        public List<CommonJsScriptModel> Scripts { get; set; }
    }
}