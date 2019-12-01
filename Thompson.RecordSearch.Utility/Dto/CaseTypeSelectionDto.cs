using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class Option
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
    }

    public class DropDown
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public List<Option> Options { get; set; }
    }

    public class CaseSearchType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
    }

    public class CaseTypeSelectionDto
    {
        public List<DropDown> DropDowns { get; set; }
        public List<CaseSearchType> CaseSearchTypes { get; set; }

        public static CaseTypeSelectionDto GetDto(string fileSuffix)
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
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CaseTypeSelectionDto>(data);
        }
    }


}
