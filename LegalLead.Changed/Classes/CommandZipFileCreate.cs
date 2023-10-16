// CommandZipFileCreate
// CommandZipVersionFolderCreate
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace LegalLead.Changed.Classes
{
    public class CommandZipFileCreate : CommandZipVersionFolderCreate
    {
        public override int Index => -700;

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(SourceFile))
            {
                throw new InvalidOperationException();
            }

            if (LatestVersion == null)
            {
                return true;
            }
            var sourceDirectory = GetSourceDirectoryName();
            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException();
            }
            var zipFile = TargetFileName;
            if (IsBeta && File.Exists(zipFile))
            {
                // recreate the beta zip every time we build
                File.Delete(zipFile);
            }
            if (!File.Exists(zipFile))
            {
                if (!IsBeta)
                {
                    // delete any files in data folder, when building the release version
                    var dataDir = Path.Combine(SourceDirectory,
                                ConfigurationManager.AppSettings["LatestVersion.Data"]);
                    var dataInfo = new DirectoryInfo(dataDir).GetFiles().ToList();
                    dataInfo.ForEach(f => f.Delete());
                }
                ZipFile.CreateFromDirectory(SourceDirectory, zipFile);
            }
            return true;
        }

        protected bool IsBeta => SourceExeVersion.Contains("Future");

        protected string SourceDirectory
        {
            get { return _srcDirectoryName ?? (_srcDirectoryName = GetSourceDirectoryName()); }
        }

        protected string TargetFileName
        {
            get { return _targetFileName ?? (_targetFileName = GetTargetFileName()); }
        }

        private string _srcDirectoryName;
        private string _targetFileName;
        private string _srcFileVersion;

        protected string SourceExeVersion
        {
            get { return _srcFileVersion ?? (_srcFileVersion = GetFileVersion()); }
        }
        private string GetSourceDirectoryName()
        {
            var solutionDir = Path.GetDirectoryName(SourceFile);
            var programName = ConfigurationManager.AppSettings["LatestVersion.Source"];
            return Path.Combine(solutionDir, programName);
        }
        private string GetTargetFileName()
        {
            // when the EXE version is Future then add BETA to file name
            var betaStamp = IsBeta ? "_Beta" : "";
            var fileName = $"{new DirectoryInfo(ProjectDirectory).Name}{betaStamp}.zip";
            return Path.Combine(ProjectDirectory, fileName);
        }


        private string GetFileVersion()
        {
            var exeFile = ConfigurationManager.AppSettings["LatestVersion.File"];
            var executableName = Path.Combine(SourceDirectory, exeFile);
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(executableName);
            return fvi.FileVersion;
        }
    }
}
