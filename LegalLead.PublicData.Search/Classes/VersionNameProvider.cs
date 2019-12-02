using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalLead.PublicData.Search.Classes
{
    public class VersionNameProvider
    {
        private static string _fileVersion;

        private static string FileVersion 
        {
            get 
            {
                return _fileVersion ?? (_fileVersion = GetFileVersion());
            } 
        }

        private static string GetFileVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }
        public VersionNameProvider()
        {
            // when the assembly-file-version contains pre-release
            var isPreRelease = FileVersion.EndsWith(".Future");
            Name = isPreRelease ? "Future" : "Default";
        }
        public string Name { get; private set; }

    }
}
