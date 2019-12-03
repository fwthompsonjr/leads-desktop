// CommandZipVersionFolderCreate
using System;
using System.Configuration;
using System.IO;

namespace LegalLead.Changed.Classes
{
    public class CommandZipVersionFolderCreate : CommandZipDirectoryCreate 
    {
        public override int Index => 600;

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(SourceFile))
                throw new ArgumentOutOfRangeException("SourceFile");

            if (LatestVersion == null)
            {
                return true;
            }
            var solutionDir = ZipDirectoryName;
            if (!Directory.Exists(solutionDir))
            {
                throw new DirectoryNotFoundException("Project zip directory is not accessible");
            }
            var projectDir = ProjectDirectory;
            if (!Directory.Exists(projectDir))
            {
                Directory.CreateDirectory(projectDir);
            }
            return true;
        }

        protected string ProjectDirectory
        {
            get { return _projectDirectoryName ?? (_projectDirectoryName = GetProjectDirectoryName()); }
        }

        private string _projectDirectoryName;

        private string GetProjectDirectoryName()
        {
            var solutionDir = ZipDirectoryName;
            var programName = ConfigurationManager.AppSettings["LatestVersion.Name"];
            var folderName = $"{programName}_{LatestVersion.Number.Replace(".", "_")}";
            return Path.Combine(solutionDir, folderName);

        }

    }
}
