using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.Dto
{

    using JConn = Newtonsoft.Json.JsonConvert;

    public class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsVerified { get; set; }
    }

    public class WebDrivers
    {
        public int SelectedIndex { get; set; }
        public IList<Driver> Drivers { get; set; }
    }

    public class WebDriverDto
    {
        public WebDrivers WebDrivers { get; set; }
    }


    public static class WebDriverDtoExtensions
    {
        private static string _dtoContent;
        private static string DtoContent => _dtoContent ?? (_dtoContent = Get());
        private static string Get()
        {
            var dataFile = DataFile();
            return File.ReadAllText(dataFile);
        }

        public static WebDriverDto Get(this WebDriverDto dto)
        {
            if (dto == null) return dto;
            var content = DtoContent;
            return JConn.DeserializeObject<WebDriverDto>(content);
        }
        public static WebDriverDto Save(this WebDriverDto dto)
        {
            if (dto == null) return dto;
            if (dto.WebDrivers == null) return dto;
            if (dto.WebDrivers.Drivers == null) throw new ArgumentOutOfRangeException(nameof(dto));
            if (dto.WebDrivers.Drivers.Count == 0) throw new ArgumentOutOfRangeException(nameof(dto));
            if (dto.WebDrivers.SelectedIndex < 0) throw new ArgumentOutOfRangeException(nameof(dto));
            if (dto.WebDrivers.SelectedIndex > dto.WebDrivers.Drivers.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(dto));
            var dataFile = DataFile();
            if (File.Exists(dataFile)) File.Delete(dataFile);

            using (StreamWriter sw = new StreamWriter(dataFile))
            {
                sw.Write(JConn.SerializeObject(dto, Newtonsoft.Json.Formatting.Indented));
            }
            _dtoContent = null;
            if (string.IsNullOrEmpty(DtoContent))
            {
                throw new FileLoadException();
            }
            return dto;
        }

        private static string DataFile()
        {

            const string fileSuffix = "webDrivers";
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

        internal static readonly string SearchSettingFileNotFound
            = CommonKeyIndexes.SearchSettingFileNotFound;
    }
}
