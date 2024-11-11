using Newtonsoft.Json;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class PermissionMapDto
    {
        public int Id { get; set; } = -1;
        [JsonProperty("webIndexes")] public string WebPermissions { get; set; } = string.Empty;
        [JsonProperty("countyId")] public string CountyPermission { get; set; } = string.Empty;
    }
}
