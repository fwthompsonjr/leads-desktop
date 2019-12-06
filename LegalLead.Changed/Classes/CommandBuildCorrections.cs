// CommandBuildCorrections
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LegalLead.Changed.Models;

namespace LegalLead.Changed.Classes
{
    public class CommandBuildCorrections : BuildCommandBase
    {
        public override int Index => 1000;

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
            var corrections = Log.Corrections ?? new List<Correction>();
            var issues = new List<Issue>();
            var issueList
                = Log.Changes
                .Select(c => c.Issues)
                .ToList();
            issueList.ForEach(x => 
            {
                var children = x.ToList();
                children.ForEach(c => 
                { 
                    if (!issues.Contains(c) && c.IsFixed && !corrections.Any(d => d.Id == c.Id)) issues.Add(c); 
                });
            });
            issues.ForEach(a =>
            {
            corrections.Add(new Correction 
            {
                Id = a.Id,
                CorrectionDate = DateTime.Now,
                Description = string.Join(" ", a.Description)
            });
            });
            if (!issues.Any()) return true;
            Log.Corrections = corrections;
            ReSerialize();
            return true;
        }

        protected void MapChange(List<Fix> fixes)
        {
            if (fixes == null) return;
            if (!fixes.Any()) return;
            fixes.ForEach(UpdateChange);
        }

        protected virtual void UpdateChange(Fix objChange)
        {
            if(objChange == null)
            {
                throw new ArgumentNullException(nameof(objChange));
            }
            var issueList = Log.Changes
                .Where(c => c.Issues.Any(x => x.Id == objChange.Id))
                .ToList();
            if (!issueList.Any()) return;
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
            if(issue == null)
            {
                throw new ArgumentNullException(nameof(issue));
            }
            var startTime = issue.Description.FirstOrDefault(a => a.StartsWith(startDate, StringComparison.InvariantCultureIgnoreCase));

            if (startTime != null) return;
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
