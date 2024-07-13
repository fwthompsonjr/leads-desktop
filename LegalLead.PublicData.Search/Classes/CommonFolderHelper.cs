using System;
using System.Collections.Generic;
using System.IO;

namespace LegalLead.PublicData.Search.Classes
{
    internal static class CommonFolderHelper
    {
        const string prefix = "data_rqst_";
        public static string MoveToCommon(string originalFileName)
        {
            if (string.IsNullOrEmpty(CommonFolder)) { return originalFileName; }
            if (!File.Exists(originalFileName)) { return originalFileName; }
            var shortName = Path.GetFileName(originalFileName);
            if (shortName == null) {  return originalFileName; }
            if (shortName.StartsWith(prefix, StringComparison.Ordinal)) shortName = shortName.Replace(prefix, string.Empty);
            var fullName = Path.Combine(CommonFolder, shortName);
            if (fullName.Equals(originalFileName, StringComparison.OrdinalIgnoreCase)) { return originalFileName; }
            if (File.Exists(fullName)) { return fullName; }
            File.Copy(originalFileName, fullName, true);
            return fullName;
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
    }
}
