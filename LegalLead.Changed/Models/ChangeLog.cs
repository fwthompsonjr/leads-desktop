using System;
using System.Collections.Generic;

namespace LegalLead.Changed.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only",
        Justification = "This field needs to be read/write to allow proper serialization.")]
    public class Issue
    {
        public double Id { get; set; }
        public string Name { get; set; }
        public bool IsFixed { get; set; }
        public int FixVersion { get; set; }
        public IList<string> Description { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only",
        Justification = "This field needs to be read/write to allow proper serialization.")]
    public class Correction
    {
        public double Id { get; set; }
        
        public DateTime CorrectionDate { get; set; }

        public string Description { get; set; }
    }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", 
        Justification = "This field needs to be read/write to allow proper serialization.")]
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only",
        Justification = "This field needs to be read/write to allow proper serialization.")]
    public class Version
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public IList<Fix> Fixes { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only",
        Justification = "This field needs to be read/write to allow proper serialization.")]
    public class ChangeLog
    {
        public IList<Change> Changes { get; set; }
        
        public IList<Version> Versions { get; set; }


        public IList<Correction> Corrections { get; set; }

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
