using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Harris.Criminal.Db.Entities
{
    public class HccProgress
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }


    public class HccMessage
    {
        [JsonProperty("dt")]
        public DateTime Date { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("progress")]
        public HccProgress Progress { get; set; }
    }

    public class HccProcess
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime? EndTime { get; set; }

        [JsonProperty("messages")]
        public List<HccMessage> Messages { get; set; }
    }
}
