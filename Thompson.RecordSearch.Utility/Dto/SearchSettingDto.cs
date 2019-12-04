using System.Collections.Generic;
using System.IO;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

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

        // private static NavInstruction _criminalInstructions;

        public static NavInstruction GetNonCriminalMapping()
        {
            var dataFile = NonCriminalMappingFile();
            var data = File.ReadAllText(dataFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<NavInstruction>(data);

        }
        public static NavInstruction GetCriminalMapping()
        {
            // if (_criminalInstructions != null) return _criminalInstructions;
            var dataFile = CriminalMappingFile();
            var data = File.ReadAllText(dataFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<NavInstruction>(data);
            // _criminalInstructions = Newtonsoft.Json.JsonConvert.DeserializeObject<NavInstruction>(data);
            // return _criminalInstructions;
        }

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

        private static string CriminalMappingFile()
        {

            const string fileSuffix = "dentonCaseCustomInstruction";
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

        private static string NonCriminalMappingFile()
        {

            const string fileSuffix = "dentonCaseCustomInstruction_1";
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
