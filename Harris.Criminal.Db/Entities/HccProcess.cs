using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Harris.Criminal.Db.Entities
{

    public static class HccStatus
    {
        public const int Verbose = 0;
        public const int Information = 10;
        public const int Warning = 100;
        public const int Error = 1000;
    }

    public class HccProgress
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }


    public class HccMessage
    {
        [JsonProperty("sts")]
        public int StatusId { get; set; }

        [JsonProperty("dt")]
        public DateTime Date { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("progress")]
        public HccProgress Progress { get; set; }
    }

    public class HccProcess
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime? EndTime { get; set; }

        [JsonProperty("messages")]
        public List<HccMessage> Messages { get; set; }


        public static List<HccProcess> Read()
        {
            var data = DataProcess.Read();
            if (string.IsNullOrEmpty(data)) return new List<HccProcess>();
            return JsonConvert.DeserializeObject<List<HccProcess>>(data);
        }

        public static List<HccProcess> Update(List<HccProcess> options)
        {
            var data = JsonConvert.SerializeObject(options);
            if (string.IsNullOrEmpty(data)) return null;
            DataProcess.Write(data);
            return Read();
        }
        public static List<HccProcess> Update(HccProcess process)
        {
            var data = Read();
            var index = data.FindIndex(a => a.Id == process.Id);
            if (index < 0)
            {
                data.Add(process);
            }
            else
            {
                data[index] = process;
            }
            
            return Update(data);
        }

        public static HccProcess LastOrDefault(string processName)
        {
            var data = Read();
            return data?.LastOrDefault(d =>
            !d.EndTime.HasValue &&
            d.Name.Equals(processName, StringComparison.OrdinalIgnoreCase));
        }

        public static HccProcess Begin(string processName)
        {
            var data = Read();
            // if there is an existing running process..
            var process = LastOrDefault(processName);
            return process ?? new HccProcess
            {
                Id = data.Count + 1,
                Name = processName, 
                StartTime = DateTime.Now 
            };
        }
        public static HccProcess End(string processName)
        {
            // if there is an existing running process..
            var process = LastOrDefault(processName);
            if (process == null) return null;
            process.EndTime = DateTime.Now;
            return process;
        }

        public static HccProcess End(string processName, Exception exception)
        {
            // if there is an existing running process..
            var process = End(processName);
            if (process == null) return null;

            if (process.Messages == null) process.Messages = new List<HccMessage>();
            process.Messages.Add(new HccMessage
            {
                Date = DateTime.Now,
                Comment = $"{processName} sub-process completed with error: {exception.Message}",
                StatusId = HccStatus.Error
            });
            Update(process);
            return process;
        }
    }
}
