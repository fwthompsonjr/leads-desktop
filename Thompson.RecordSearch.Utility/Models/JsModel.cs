using Newtonsoft.Json;

namespace Thompson.RecordSearch.Utility.Models
{
    public class JsModel
    {
        [JsonProperty("name")] public string Name { get; set; } = string.Empty;
        [JsonProperty("code")] public string Code { get; set; } = string.Empty;
        [JsonProperty("userId")] public string UserId { get; set; } = string.Empty;
    }
}
