using Newtonsoft.Json;
using System;

namespace Thompson.RecordSearch.Utility.Models
{
    public class BillTypeHistoryModel
    {
        [JsonProperty("id")] public string Id { get; set; } = string.Empty;
        [JsonProperty("userPermissionId")] public string UserPermissionId { get; set; } = string.Empty;
        [JsonProperty("userId")] public string UserId { get; set; } = string.Empty;
        [JsonProperty("permissionMapId")] public string PermissionMapId { get; set; } = string.Empty;
        [JsonProperty("keyName")] public string KeyName { get; set; } = string.Empty;
        [JsonProperty("keyValue")] public string KeyValue { get; set; } = string.Empty;
        [JsonProperty("groupId")] public int? GroupId { get; set; }
        [JsonProperty("createDate")] public DateTime? CreateDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
    }
}
