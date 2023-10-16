using Newtonsoft.Json;
using System.Collections.Generic;

namespace Harris.Criminal.Db.Tables
{
    public class ReferenceTable
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        public List<ReferenceDatum> Data { get; set; }
    }
}
