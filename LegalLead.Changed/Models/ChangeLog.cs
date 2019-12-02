using System;
using System.Collections.Generic;

namespace LegalLead.Changed.Models
{
    public class Issue
    {
        public double Id { get; set; }
        public string Name { get; set; }
        public bool IsFixed { get; set; }
        public int FixVersion { get; set; }
        public IList<string> Description { get; set; }
    }

    public class Change
    {
        public DateTime ReportedDate { get; set; }
        public IList<Issue> Issues { get; set; }
    }

    public class Fix
    {
        public double Id { get; set; }
        public string Description { get; set; }
        public bool CanPublish { get; set; }
    }

    public class Version
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public IList<Fix> Fixes { get; set; }
    }

    public class ChangeLog
    {
        public IList<Change> Changes { get; set; }
        
        public IList<Version> Versions { get; set; }

        /// <summary>
        /// Gets or sets the string content for the version assembly file
        /// </summary>
        public IList<string> Template { get; set; }

        /// <summary>
        /// Gets or sets the relative name of the version assembly template file
        /// </summary>
        public string TemplateFile { get; set; }
    }


}
