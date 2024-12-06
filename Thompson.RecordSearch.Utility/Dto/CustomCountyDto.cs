using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class CustomCountyDto
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("county")] public string CountyName { get; set; } = string.Empty;
        [JsonProperty("notes")] public string[] Notes { get; set; } = Array.Empty<string>();

        public static List<CustomCountyDto> GetNotes()
        {
            var js = Properties.Resources.custom_county_notes;
            return JsonConvert.DeserializeObject<List<CustomCountyDto>>(js) ?? new List<CustomCountyDto>();
        }
    }
}
