using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.Dto
{


    public class Example
    {
        public SearchSettingDto SearchSetting { get; set; }
    }

    public class SearchSettingDto
    {

        public int CountySearchTypeId { get; set; }
        public int CountyCourtId { get; set; }
        public int DistrictCourtId { get; set; }
        public int DistrictSearchTypeId { get; set; }


        public static SearchSettingDto GetDto()
        {
            var dataFile = DataFile();
            var data = File.ReadAllText(dataFile);
            var parent = Newtonsoft.Json.JsonConvert.DeserializeObject<Example>(data);
            return parent.SearchSetting;
        }

        public static void Save(SearchSettingDto source)
        {

            var dataFile = DataFile();
            var parent = new Example { SearchSetting = source };
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(parent, Newtonsoft.Json.Formatting.Indented);
            File.Delete(dataFile);
            using(StreamWriter sw = new StreamWriter(dataFile))
            {
                sw.Write(data);
            }

        }

        private static string DataFile()
        {

            const string fileSuffix = "denton-settings";
            const string dataFormat = @"{0}\xml\{1}.json";
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(dataFormat,
                appDirectory,
                fileSuffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException("Unable to find search setings access json");
            }
            return dataFile;
        }
    }
}
