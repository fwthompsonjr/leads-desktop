using System;
using System.Configuration;
using System.IO;

namespace LegalLead.Changed.Classes
{
    public class CommandTransferReadMe : BuildCommandBase
    {
        public override int Index => 1300;

        public override bool Execute()
        {
            var isDebug = false;
            const string allowIt = "true";
            var allMapCorrection = ConfigurationManager
                .AppSettings["Allow.ReadMe.Corrections"] ?? allowIt;
            var allowExec = allMapCorrection.Equals(allowIt,
                StringComparison.CurrentCulture);
            if (!allowExec) return false;

#if DEBUG
            isDebug = true;
#endif

            Console.WriteLine("Debug: ReadMe.txt to Application Folder");
            var sourceFileName = ReadMeFileName();
            var targetFileName = SolutionReadMe();

            if (!File.Exists(sourceFileName))
                throw new FileNotFoundException(sourceFileName);

            if (!File.Exists(targetFileName))
                throw new FileNotFoundException(targetFileName);

            var sourceContent = string.Empty;
            var targetContent = string.Empty;
            using (var contentReader = new StreamReader(sourceFileName))
            {
                sourceContent = contentReader.ReadToEnd();
            }
            using (var targetReader = new StreamReader(targetFileName))
            {
                targetContent = targetReader.ReadToEnd() ?? string.Empty;
            }

            if (sourceContent.Equals(targetContent,
                StringComparison.CurrentCultureIgnoreCase)) return true;

            if (!isDebug) return false;

            using (var writer = new StreamWriter(targetFileName))
            {
                writer.Write(sourceContent);
            }
            return true;

        }
    }
}
