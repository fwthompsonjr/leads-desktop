using System;
using System.Linq;
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
            var issueList = Log.Changes
                .Where(c => c.Issues.Any(x => x.Id == obj.Id & !x.IsFixed))
                .ToList();

            if (!issueList.Any()) return;

            foreach (var item in issueList)
            {
                var targets = item.Issues
                    .Where(x => x.Id == obj.Id && x.FixVersion == LatestVersion.Id)
                    .ToList();
                targets.ForEach(MarkAsStarted);
            }
        }

    }
}
