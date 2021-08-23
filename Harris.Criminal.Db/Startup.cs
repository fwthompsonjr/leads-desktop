using Harris.Criminal.Db.Downloads;
using Harris.Criminal.Db.Tables;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Harris.Criminal.Db
{
    public static class Startup
    {

        internal static string _appFolder;
        internal static string AppFolder => _appFolder ?? (_appFolder = GetAppFolderName());

        /// <summary>
        /// Gets the name of the application directory.
        /// </summary>
        /// <returns></returns>
        private static string GetAppFolderName()
        {
            var execName = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            return Path.GetDirectoryName(execName);
        }

        public static void Read()
        {
            References.Read();
            Downloads.Read();
        }

        public static class Downloads
        {
            private enum FileDateType
            {
                Max,
                Min
            }
            private static string _dataFolder;

            private static string DataFolder => _dataFolder ?? (_dataFolder = GetDataFolderName());
            /// <summary>
            /// Gets the name of the application data directory.
            /// </summary>
            /// <returns></returns>
            private static string GetDataFolderName()
            {
                var parentName = AppFolder;
                return Path.Combine(parentName, "_db", "_downloads");
            }

            public static List<string> FileNames { get; private set; }

            public static List<HarrisCountyListDto> DataList { get; private set; }

            public static void Read()
            {
                const string extn = "*CrimFilingsWithFutureSettings*.txt";
                var directory = new DirectoryInfo(DataFolder);
                var files = directory.GetFiles(extn).ToList();
                FileNames = files.Select(f => f.FullName).ToList();
                var records = new List<HarrisCountyListDto>();
                foreach (var item in files)
                {
                    var data = HarrisCriminalDto.Map(item.FullName);
                    records.Add(new HarrisCountyListDto
                    {
                        CreateDate = item.CreationTime,
                        FileDate = GetFileDate(item.CreationTime, data),
                        MaxFilingDate = GetMinOrMax(DateTime.MinValue, data, FileDateType.Max),
                        MinFilingDate = GetMinOrMax(DateTime.MinValue, data, FileDateType.Min),
                        Name = Path.GetFileNameWithoutExtension(item.Name),
                        Data = data
                    });
                }
                DataList = records;
            }

            private static DateTime GetMinOrMax(DateTime minValue, List<HarrisCriminalDto> data, FileDateType dateType)
            {
                if (data == null || !data.Any())
                {
                    return minValue;
                }
                DateTime lookupDate = minValue;
                switch (dateType)
                {
                    case FileDateType.Max:
                        lookupDate = data.Max(m =>
                        {
                            var filingDate = m.FilingDate;
                            if (DateTime.TryParseExact(
                                filingDate,
                                "yyyyMMdd",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeLocal,
                                out DateTime mxDate))
                            {
                                return mxDate;
                            }
                            return minValue;
                        });
                        break;
                    case FileDateType.Min:
                        lookupDate = data.Min(m =>
                        {
                            var fileDate = m.FilingDate;
                            if (DateTime.TryParseExact(
                                fileDate,
                                "yyyyMMdd",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeLocal,
                                out DateTime mnDate))
                            {
                                return mnDate;
                            }
                            return DateTime.MaxValue;
                        });
                        if (lookupDate.Equals(DateTime.MaxValue))
                        {
                            lookupDate = minValue;
                        }
                        break;
                }
                return lookupDate;
            }

            private static DateTime GetFileDate(DateTime creationTime, List<HarrisCriminalDto> data)
            {
                if (data == null || !data.Any())
                {
                    return creationTime;
                }
                var datum = data.FirstOrDefault();
                if (datum == null)
                {
                    return creationTime;
                }
                if (DateTime.TryParse(datum.DateDatasetProduced, out DateTime dataDate))
                {
                    return dataDate;
                }
                return creationTime;
            }
        }

        public static class References
        {
            private static string _dataFolder;

            private static string DataFolder => _dataFolder ?? (_dataFolder = GetDataFolderName());
            /// <summary>
            /// Gets the name of the application data directory.
            /// </summary>
            /// <returns></returns>
            private static string GetDataFolderName()
            {
                var parentName = AppFolder;
                return Path.Combine(parentName, "_db", "_tables");
            }

            public static List<string> FileNames { get; private set; }

            public static List<ReferenceTable> DataList { get; private set; }

            public static void Read()
            {
                const string extn = "*hcc.tables.*.json";
                var directory = new DirectoryInfo(DataFolder);
                var files = directory.GetFiles(extn).ToList();
                FileNames = files.Select(f => f.FullName).ToList();
                var tables = new List<ReferenceTable>();
                FileNames.ForEach(f => { tables.Add(Read<ReferenceTable>(f)); });
                DataList = tables;
            }

            private static T Read<T>(string sourceFileName) where T : class
            {
                var content = GetFileContent(sourceFileName);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
            }

            private static string GetFileContent(string sourceFileName)
            {
                using (var reader = new StreamReader(sourceFileName))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
