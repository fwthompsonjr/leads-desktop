using LegalLead.Changed.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LegalLead.Changed.Classes
{
    public class CommandMapFixes : BuildCommandBase
    {
        public override int Index => 300;

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(SourceFile))
            {
                throw new InvalidOperationException();
            }

            if (Log == null)
            {
                throw new InvalidOperationException();
            }

            if (LatestVersion == null)
            {
                return true;
            }

            var fixes = LatestVersion.Fixes
                .Where(x => x.Id > 0)
                .ToList();

            if (!fixes.Any())
            {
                return true;
            }

            MapChange(fixes);
            ReSerialize();
            return true;
        }

        protected void MapChange(List<Fix> fixes)
        {
            if (fixes == null)
            {
                return;
            }

            if (!fixes.Any())
            {
                return;
            }

            fixes.ForEach(UpdateChange);
        }

        protected virtual void UpdateChange(Fix objChange)
        {
            if (objChange == null)
            {
                throw new ArgumentNullException(nameof(objChange));
            }
            var issueList = Log.Changes
                .Where(c => c.Issues.Any(x => x.Id == objChange.Id))
                .ToList();
            if (!issueList.Any())
            {
                return;
            }

            foreach (var item in issueList)
            {
                var targets = item.Issues
                    .Where(x => x.Id == objChange.Id && x.FixVersion == 0)
                    .ToList();
                targets.ForEach(x =>
                {
                    x.FixVersion = LatestVersion.Id;
                    Console.WriteLine("Associating issue {0} -- [ {1} ] to Version {2}",
                        x.Id.ToString("F3", new NumberFormatInfo()),
                        x.Name,
                        LatestVersion.Number
                        );
                });
            }
        }



        protected static void MarkAsStarted(Issue issue)
        {
            if (issue == null)
            {
                throw new ArgumentNullException(nameof(issue));
            }
            var startTime = issue.Description.FirstOrDefault(a => a.StartsWith(startDate, StringComparison.InvariantCultureIgnoreCase));

            if (startTime != null)
            {
                return;
            }

            var timeStamp = DateTime.Now.ToString("u");
            Console.WriteLine("Starting issue {0} -- [ {1} ] at {2}",
                issue.Id.ToString("F3"),
                issue.Name,
                timeStamp
                );
            issue.Description.Add($@"{startDate}{timeStamp}");
        }


        const string startDate = @"Start Date: ";
    }
}
