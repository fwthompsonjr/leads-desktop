using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegalLead.Changed.Models;

namespace LegalLead.Changed.Classes
{
    public class CommandFixCanPublish : CommandMapFixes
    {
        public override int Index => 400;

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(SourceFile))
                throw new ArgumentOutOfRangeException("SourceFile");

            if (LatestVersion == null)
            {
                return true;
            }
            // we do this to pick up any change from prior command
            ResetFileSource(SourceFile);
            var fixes = LatestVersion.Fixes
                .Where(x => x.Id > 0 & !x.CanPublish)
                .ToList();

            MapChange(fixes);
            ReSerialize();
            return true;
        }

        protected override void UpdateChange(Fix obj)
        {
            const string changing = @"Start Date: ";
            var issueList = Log.Changes
                .Where(c => c.Issues.Any(x => x.Id == obj.Id & !x.IsFixed))
                .ToList();

            if (!issueList.Any()) return;

            foreach (var item in issueList)
            {
                var targets = item.Issues
                    .Where(x => x.Id == obj.Id && x.FixVersion == LatestVersion.Id)
                    .ToList();
                targets.ForEach(x =>
                {
                    var startTime = x.Description.FirstOrDefault(a => a.StartsWith(changing));
                    if (startTime != null) return;
                    var timeStamp = DateTime.Now.ToString("u");
                    Console.WriteLine("Starting issue {0} -- [ {1} ] at {2}",
                        x.Id.ToString("F3"),
                        x.Name,
                        timeStamp
                        );
                    x.Description.Add($@"{changing}{timeStamp}");
                });
            }
        }
    }
}
