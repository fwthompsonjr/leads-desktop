using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Thompson.RecordSearch.Utility.Db
{
    public class AddressItemDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")] 
        public string Name { get; set; }
        [JsonProperty("address")]
        public IEnumerable<string> Address { get; set; } = Array.Empty<string>();
    }
}
