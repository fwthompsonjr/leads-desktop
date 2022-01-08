using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db = Harris.Criminal.Db;
namespace LegalLead.PublicData.Search.Classes
{
    public class HccOption
    {
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

        public static List<HccOption> Read()
        {
            var data = Db.DataOptions.Read();
            if (string.IsNullOrEmpty(data)) return null;
            return JsonConvert.DeserializeObject<List<HccOption>>(data);
        }

        public static List<HccOption> Update(List<HccOption> options)
        {
            var data = JsonConvert.SerializeObject(options);
            if (string.IsNullOrEmpty(data)) return null;
            Db.DataOptions.Write(data);
            return Read();
        }
    }
}
