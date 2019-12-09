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
            var corrections = (Log.Corrections ?? new List<Correction>()).ToList();
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
            corrections.Sort((a, b) => a.Id.CompareTo(b.Id));
            Log.Corrections = corrections;
            ReSerialize();
            return true;
        }

    }
}
