using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class HccOptionDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("labels")]
        public List<string> Labels { get; set; }

        [JsonProperty("index")]
        public int? Index { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("values")]
        public List<string> Values { get; set; }

    }
}
