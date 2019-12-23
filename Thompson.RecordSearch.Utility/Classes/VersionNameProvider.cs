using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class VersionNameProvider
    {
        public static List<string> VersionNames
        {
            get
            {
                return _versionNames ?? (_versionNames = GetNames());
            }
        }

        public static string FileVersion
        {
            get 
            {
                return _fileVersion ?? (_fileVersion = GetFileVersion());
            } 
        }

        public VersionNameProvider()
        {
            // when the assembly-file-version contains pre-release
            var isPreRelease = FileVersion.EndsWith($"~{VersionNames.Last()}", 
                System.StringComparison.CurrentCultureIgnoreCase);
            Name = isPreRelease ? 
                CommonKeyIndexes.FutureKeyWord : 
                CommonKeyIndexes.DefaultKeyWord;
        }
        public string Name { get; private set; }


        private static string GetFileVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

        private static List<string> GetNames()
        {
            return new List<string> { 
                CommonKeyIndexes.DefaultKeyWord,
                CommonKeyIndexes.FutureKeyWord };
        }
        private static string _fileVersion;
        private static List<string> _versionNames;

    }
}
