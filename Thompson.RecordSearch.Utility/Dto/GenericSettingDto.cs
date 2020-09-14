using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class GenericSettingDto : SearchSettingDto
    {
        private static Dictionary<string, GenericSetting> _settings;


        public GenericSettingDto(string name)
        {
            if(_settings == null) { 
                _settings = new Dictionary<string, GenericSetting>(); 
            }
            if (!_settings.ContainsKey(name))
            {
                _settings.Add(name, new GenericSetting { Name = name });
            }
            Name = name;
        }

        public string Name { get; private set; }

        public new void Save(SearchSettingDto source)
        {
            _settings[Name].Save(source);
        }

        public new SearchSettingDto GetDto()
        {
            return _settings[Name].GetDto();
        }

    }

    public class GenericSetting
    {
        private string _name;
        public string Name {
            get { return _name; }
            set
            {
                _name = value;
                var fileSuffix = value;
                var searchSettingFileNotFound = CommonKeyIndexes.SearchSettingFileNotFound;
                const string dataFormat = @"{0}\xml\{1}.json";
                var appDirectory = ContextManagment.AppDirectory;
                var dataFile = string.Format(
                    CultureInfo.CurrentCulture,
                    dataFormat,
                    appDirectory,
                    fileSuffix);
                if (!File.Exists(dataFile))
                {
                    throw new FileNotFoundException(searchSettingFileNotFound);
                }

                DataFile = dataFile;
                Content = File.ReadAllText(dataFile);
            } 
        }

        public string DataFile { get; private set; }

        public string Content { get; private set; }

        public SearchSettingDto GetDto()
        {
            var parent = Newtonsoft.Json.JsonConvert.DeserializeObject<Example>(Content);
            return parent.SearchSetting;

        }

        public void Save(SearchSettingDto source)
        {

            var dataFile = DataFile;
            var parent = new Example { SearchSetting = source };
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(parent, 
                Newtonsoft.Json.Formatting.Indented);
            if (File.Exists(dataFile)) { File.Delete(dataFile); }
            using (StreamWriter sw = new StreamWriter(dataFile))
            {
                sw.Write(data);
            }
            Content = File.ReadAllText(dataFile);

        }
    }
}
