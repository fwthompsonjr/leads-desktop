using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace Thompson.RecordSearch.Utility.Db
{
    public static class DataPersistence
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

        private static string _dataFolder;

        /// <summary>
        /// Gets the data folder.
        /// </summary>
        /// <value>
        /// The data folder.
        /// </value>
        public static string DataFolder => _dataFolder ?? (_dataFolder = GetDataFolderName());

        /// <summary>
        /// Gets the name of the application data directory.
        /// </summary>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private static string GetDataFolderName()
        {
            var parentName = AppFolder;
            var dataFolder = Path.Combine(parentName, "_db", "_downloads");
            if (Directory.Exists(dataFolder))
                return dataFolder;

            Directory.CreateDirectory(dataFolder);
            return dataFolder;
        }

        public static T GetContent<T>(string fileName) where T : class
        {
            if (!FileExists(fileName))
            {
                return default;
            }
            var targetFile = Path.Combine(DataFolder, fileName);
            using (var reader = new StreamReader(targetFile))
            {
                var content = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        public static void Save(string fileName, object data)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var targetFile = Path.Combine(DataFolder, fileName);
            if (File.Exists(targetFile))
                throw new ArgumentOutOfRangeException(nameof(fileName));

            var content = JsonConvert.SerializeObject(data);
            using (var writer = new StreamWriter(targetFile))
            {
                writer.Write(content);
                writer.Close();
            }
        }

        public static bool FileExists(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));


            var targetFile = Path.Combine(DataFolder, fileName);
            return File.Exists(targetFile);
        }
    }
}
