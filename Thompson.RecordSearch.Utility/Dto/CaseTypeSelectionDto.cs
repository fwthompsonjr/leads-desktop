using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.Dto
{
#pragma warning disable CA1716 // Identifiers should not match keywords
    public class Option
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }

        public bool IsDefault { get; set; }
    }

    public class DropDown
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public List<DropDown> DropDowns { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public List<CaseSearchType> CaseSearchTypes { get; set; }

        public static CaseTypeSelectionDto GetDto(string fileSuffix)
        {

            const string dataFormat = @"{0}\xml\{1}.json";
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                fileSuffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException(CommonKeyIndexes.NavigationFileNotFound);
            }
            var data = File.ReadAllText(dataFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CaseTypeSelectionDto>(data);
        }
    }

    public static class DropDownOptionExtensions
    {
        public static List<DropDown> ToDropDown(this List<Option> options)
        {
            if (options == null) return null;
            var result = new List<DropDown>();
            options.ForEach(o =>
            {
                result.Add(new DropDown { Id = o.Id, Name = o.Name, Query = o.Query });
            });
            return result;

        }
    }

}
