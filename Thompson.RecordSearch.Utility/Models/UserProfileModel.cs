using Newtonsoft.Json;

namespace Thompson.RecordSearch.Utility.Models
{
    public class UserProfileModel
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("orderId")] public int? OrderId { get; set; }
        [JsonProperty("userId")] public string UserId { get; set; }
        [JsonProperty("profileId")] public string ProfileId { get; set; }
        [JsonProperty("profileGroup")] public string ProfileGroup { get; set; }
        [JsonProperty("keyName")] public string KeyName { get; set; }
        [JsonProperty("keyValue")] public string KeyValue { get; set; }
    }
}
