using LegalLead.Changed.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalLead.Changed.Classes
{
    public class CommandUpdateReadMe : BuildCommandBase
    {
        public override int Index => 1200;

        public override bool Execute()
        {
            var fileName = ReadMeFileName();
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);
            // get Log
            var corrections = Log.Corrections.
                ToList()
                .FindAll(c => !c.IsWritten);
            if (!corrections.Any()) return true;
            var header = GetHeader();
            var footer = GetFooter();
            var lineSep = footer.Replace("=", "-");
            using (var writer = new StreamWriter(fileName, false))
            {
                writer.Write(header);
                var lastId = corrections.Last().Id;
                corrections.ForEach(c => 
                {
                    var change = FindChange(c.Id) 
                        ?? new Change { ReportedDate = DateTime.Now };
                    writer.Write(c.ToLogEntry(change));
                    if (c.Id != lastId)
                    {
                        writer.WriteLine(lineSep); 
                    }
                });
                writer.Write(GetFooter());
            }
            return true;
        }

        private string GetHeader()
        {
            const string readMeHeader = "ReadMe.Header";
            const string versionStamp = "{VersionNumber}";
            const string releaseStamp = "{ReleaseDate}";
            var entries = ConfigurationManager.AppSettings
                .AllKeys
                .ToList()
                .FindAll(x => x.StartsWith(readMeHeader, StringComparison.CurrentCulture));
            var builder = new StringBuilder();
            entries.ForEach(x =>
            {
                var entry = ConfigurationManager.AppSettings[x]
                .Replace(versionStamp, LatestVersion.Number)
                .Replace(releaseStamp, DateTime.Now.ToString("g"));
                if(!entry.EndsWith("|", StringComparison.CurrentCultureIgnoreCase))
                {
                    var len = 84 - entry.Length;
                    entry += string.Empty.ToFixedWidth(len);
                    entry += "|";
                }
                builder.AppendLine(entry);
             });
            return builder.ToString();
        }

        private string GetFooter()
        {
            const string readMeHeader = "ReadMe.Header.001";
            return ConfigurationManager.AppSettings[readMeHeader];
        }

        private Change FindChange(double id)
        {
            return Log.Changes
                .Where(c => c.Issues.Any(d => d.Id == id))
                .ToList()
                .FirstOrDefault();
        }
    }
}
