using Newtonsoft.Json;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Classes
{
    public class HccConfiguration
    {
        [JsonProperty("dropdown")]
        public HccConfigurationSetting Dropdown { get; set; }

        [JsonProperty("background")]
        public HccConfigurationProcess Background { get; set; }


        private static HccConfiguration _instance = null;
        public static HccConfiguration Load()
        {
            if (_instance != null) { return _instance; }
            var js = SettingsManager.CustomSettings;
            _instance = JsonConvert.DeserializeObject<HccConfiguration>(js);
            return _instance;

        }
    }
}
