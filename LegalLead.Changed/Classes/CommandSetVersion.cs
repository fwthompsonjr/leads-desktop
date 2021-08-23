using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LegalLead.Changed.Classes
{
    public class CommandSetVersion : BuildCommandBase
    {
        public override int Index => 200;

        public override bool Execute()
        {

            if (string.IsNullOrEmpty(SourceFile))
                throw new InvalidOperationException();

            if (Log == null)
                throw new InvalidOperationException();

            if (LatestVersion == null)
            {
                return true;
            }

            var sourceDir = Path.GetDirectoryName(SourceFile);
            var templateSource = $@"{sourceDir}\{Log.TemplateFile}";
            if (!File.Exists(templateSource))
            {
                return false;
            }
            Console.WriteLine("Latest Version is :=  {0}", LatestVersion.Number);
            const string versionNumber = "{VersionNumber}";
            const string fileVersionNumber = "{FileVersionNumber}";
            var isPreRelease = LatestVersion.Fixes.Any(a => !a.CanPublish);
            var futureStamp = isPreRelease ? "~Future" : string.Empty;
            var actual = File.ReadAllText(templateSource) ?? string.Empty;
            var expected = new StringBuilder(string.Join(Environment.NewLine, Log.Template))
                .Replace(versionNumber, LatestVersion.Number)
                .Replace(fileVersionNumber, $"{LatestVersion.Number}{DateTime.Now.ToString("yyyyMM")}{futureStamp}")
                .ToString();
            if (actual.Equals(expected, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return UpdateVersion(
                templateSource,
                expected);
        }

        private bool UpdateVersion(string versionCsFile, string content)
        {
            if (!File.Exists(versionCsFile))
            {
                throw new ArgumentNullException(nameof(versionCsFile));
            }
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(nameof(content));
            }
            try
            {
                using (StreamWriter writer = new StreamWriter(versionCsFile, false))
                {
                    writer.Write(content);
                }

                Console.WriteLine("Updated Assembly stamp to latest version, Number :=  {0}", LatestVersion.Number);
            }
            catch (Exception)
            {

                throw;
            }
            return true;
        }
    }
}
