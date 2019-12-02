using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegalLead.Changed.Models;

namespace LegalLead.Changed.Classes
{
    public class CommandMapFixes : BuildCommandBase
    {
        public override int Index => 300;

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(SourceFile))
                throw new ArgumentOutOfRangeException("SourceFile");

            if (Log == null)
                throw new ArgumentOutOfRangeException("Log");

            if (LatestVersion == null)
            {
                return true;
            }

            var fixes = LatestVersion.Fixes
                .Where(x => x.Id > 0)
                .ToList();

            if (!fixes.Any()) return true;
            MapChange(fixes);
            ReSerialize();
            return true;
        }

        protected void MapChange(List<Fix> fixes)
        {
            if (fixes == null) return;
            if (!fixes.Any()) return;
            fixes.ForEach(UpdateChange);
        }

        protected virtual void UpdateChange(Fix obj)
        {
            var issueList = Log.Changes
                .Where(c => c.Issues.Any(x => x.Id == obj.Id))
                .ToList();
            if (!issueList.Any()) return;
            foreach (var item in issueList)
            {
                var targets = item.Issues
                    .Where(x => x.Id == obj.Id && x.FixVersion == 0)
                    .ToList();
                targets.ForEach(x => 
                {
                    x.FixVersion = LatestVersion.Id;
                    Console.WriteLine("Associating issue {0} -- [ {1} ] to Version {2}",
                        x.Id.ToString("F3"),
                        x.Name,
                        LatestVersion.Number
                        );
                }); 
            }
        }
    }
}
