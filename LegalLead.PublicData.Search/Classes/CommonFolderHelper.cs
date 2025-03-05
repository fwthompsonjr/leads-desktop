using LegalLead.PublicData.Search.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace LegalLead.PublicData.Search.Classes
{
    public static class CommonFolderHelper
    {
        const string prefix = "data_rqst_";
        public static string MoveToCommon(string originalFileName)
        {
            if (string.IsNullOrEmpty(CommonFolder)) { return originalFileName; }
            if (!File.Exists(originalFileName)) { return originalFileName; }
            var shortName = Path.GetFileName(originalFileName);
            if (shortName == null) { return originalFileName; }
            if (shortName.StartsWith(prefix, StringComparison.Ordinal)) shortName = shortName.Replace(prefix, string.Empty);
            var fullName = Path.Combine(CommonFolder, shortName);
            if (fullName.Equals(originalFileName, StringComparison.OrdinalIgnoreCase)) { return originalFileName; }
            if (File.Exists(fullName)) { return fullName; }
            File.Copy(originalFileName, fullName, true);
            return fullName;
        }
        public static List<FileInfo> GetFiles()
        {
            var commonPath = CommonFolder; // this is from program-data
            var localPath = LocalFolder; // this is from local-app-data
            if (!Directory.Exists(commonPath) && !Directory.Exists(localPath)) { return []; }
            var files = new List<FileInfo>();
            if (Directory.Exists(commonPath))
            {
                files.AddRange(new DirectoryInfo(commonPath).GetFiles("*.xlsx"));
            }
            if (Directory.Exists(localPath))
            {
                var localFiles = new DirectoryInfo(localPath).GetFiles("*.xlsx", SearchOption.AllDirectories).ToList();
                localFiles.RemoveAll(x => files.Exists(f => f.Name.Equals(x.Name)));
                files.AddRange(localFiles);
            }
            files.RemoveAll(IsNotExcelPackage);
            files.Sort((a, b) => a.FullName.CompareTo(b.FullName));
            return files;
        }
        private static bool IsNotExcelPackage(FileInfo fileInfo)
        {
            var isValid = ExcelExtensions.IsValidExcelPackage(fileInfo.FullName);
            return !isValid;
        }

        private static string LocalFolder
        {
            get
            {
                if (localDataFolder != null) { return localDataFolder; }
                localDataFolder = GetLocalFolder();
                return localDataFolder;
            }
        }
        private static string CommonFolder
        {
            get
            {
                if (commonFolder != null) { return commonFolder; }
                commonFolder = GetCommonFolder();
                return commonFolder;
            }
        }

        private static string commonFolder;
        private static string localDataFolder = null;
        private static string GetCommonFolder()
        {
            bool ismapped = false;
            var folderName = string.Empty;
            var folders = new List<Environment.SpecialFolder>
            {
                Environment.SpecialFolder.CommonApplicationData,
                Environment.SpecialFolder.CommonDocuments
            };
            folders.ForEach(f =>
            {
                if (!ismapped)
                {
                    folderName = Environment.GetFolderPath(f);
                    if (Directory.Exists(folderName))
                    {
                        folderName = Path.Combine(folderName, "ll");
                        if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);
                        ismapped = Directory.Exists(folderName);
                    }
                }
            });
            return folderName;
        }

        private static string GetLocalFolder()
        {
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!Directory.Exists(localAppDataPath)) return string.Empty;
            string leadPath = Path.Combine(localAppDataPath, "LegalLead");
            if (!Directory.Exists(leadPath)) return string.Empty;
            return leadPath;
        }
    }
}
