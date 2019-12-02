using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalLead.Changed.Classes
{
    public class CommandSetVersion : BuildCommandBase
    {
        public override int Index => 2;

        public override bool Execute()
        {

            if (string.IsNullOrEmpty(SourceFile))
                throw new ArgumentOutOfRangeException("SourceFile");

            if (Log == null)
                throw new ArgumentOutOfRangeException("Log");

            if(LatestVersion == null)
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
            const string replacement = "{VersionNumber}";
            var actual = File.ReadAllText(templateSource) ?? string.Empty;
            var expected = (new StringBuilder(string.Join(Environment.NewLine, Log.Template)))
                .Replace(replacement, LatestVersion.Number)
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
            if(!File.Exists(versionCsFile))
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
