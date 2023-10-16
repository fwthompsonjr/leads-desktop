// CommandZipDirectoryCreate
using System;
using System.IO;

namespace LegalLead.Changed.Classes
{
    public class CommandZipDirectoryCreate : BuildCommandBase
    {
        public override int Index => 500;

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
            var solutionDir = Path.GetDirectoryName(SourceFile);
            if (!Directory.Exists(solutionDir))
            {
                throw new DirectoryNotFoundException();
            }
            var zipDir = ZipDirectoryName;
            if (!Directory.Exists(zipDir))
            {
                Directory.CreateDirectory(zipDir);
            }
            return true;
        }

        protected string ZipDirectoryName
        {
            get { return _zipDirectoryName ?? (_zipDirectoryName = GetZipDirectoryName()); }
        }

        private string _zipDirectoryName;

        private string GetZipDirectoryName()
        {
            var solutionDir = Path.GetDirectoryName(SourceFile);
            return Path.Combine(solutionDir, "_Zip");

        }

    }
}
