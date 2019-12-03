using System.Collections.Generic;
using System.IO;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class DataPoint
    {
        public string Name { get; set; }
        public string Xpath { get; set; }
        public string Result { get; set; }
    }

    public class DataPointLocatorDto
    {
        public IList<DataPoint> DataPoints { get; set; }


        public static DataPointLocatorDto GetDto(string fileSuffix)
        {

            const string dataFormat = @"{0}\xml\{1}.json";
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(dataFormat,
                appDirectory,
                fileSuffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException("Unable to find user access json");
            }
            var data = File.ReadAllText(dataFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataPointLocatorDto>(data);
        }

        public static DataPointLocatorDto Load(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataPointLocatorDto>(data);

        }
    }

}
