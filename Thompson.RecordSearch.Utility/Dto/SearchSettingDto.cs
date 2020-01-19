using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class Court
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
    }

    public class CourtLocation
    {
        public string Id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", 
            "CA2227:Collection properties should be read only", 
            Justification = "<Pending>")]
        public IList<Court> Courts { get; set; }
    }

    public class CourtLookup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage",
            "CA2227:Collection properties should be read only",
            Justification = "<Pending>")]
        public IList<CourtLocation> CourtLocations { get; set; }
    }

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

        public static CourtLookup GetCourtLookupList
        {
            get { return _courtJson ?? (_courtJson = GetCourtLookup()); }
        }

        private static CourtLookup _courtJson;

        private static CourtLookup GetCourtLookup()
        {
            var dataFile = CourtLookupFile();
            var data = File.ReadAllText(dataFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CourtLookup>(data);
        }

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
            if (File.Exists(dataFile)) { File.Delete(dataFile); }
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
            var dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                fileSuffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException(SearchSettingFileNotFound);
            }
            return dataFile;
        }

        private static string CriminalMappingFile()
        {

            const string fileSuffix = "dentonCaseCustomInstruction";
            const string dataFormat = @"{0}\xml\{1}.json";
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                fileSuffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException(SearchSettingFileNotFound);
            }
            return dataFile;
        }

        private static string NonCriminalMappingFile()
        {

            const string fileSuffix = "dentonCaseCustomInstruction_1";
            const string dataFormat = @"{0}\xml\{1}.json";
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                fileSuffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException(SearchSettingFileNotFound);
            }
            return dataFile;
        }

        private static string CourtLookupFile()
        {

            const string fileSuffix = "courtAddress";
            const string dataFormat = @"{0}\xml\{1}.json";
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                fileSuffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException(SearchSettingFileNotFound);
            }
            return dataFile;
        }

        private static readonly string SearchSettingFileNotFound 
            = CommonKeyIndexes.SearchSettingFileNotFound;
    }
}
