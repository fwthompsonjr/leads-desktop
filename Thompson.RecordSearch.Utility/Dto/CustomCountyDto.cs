using Newtonsoft.Json;
using System;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class CustomCountyDto
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("county")] public string CountyName { get; set; } = string.Empty;
        [JsonProperty("notes")] public string[] Notes { get; set; } = Array.Empty<string>();
    }
}
