using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LegalLead.Changed.Models
{

    public static class CorrectionColumnLength
    {
        public const int IssueId = 10;
        public const int ReportedDate = 15;
        public const int CorrectedDate = 16;
        public const int Comments = 34;

    }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only",
        Justification = "This field needs to be read/write to allow proper serialization.")]
    public class Issue
    {
        public string ChangeId { get; set; }

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
        const string reportedDate = "{ReportingDate}";
        public double Id { get; set; }
        
        public DateTime CorrectionDate { get; set; }

        public string Description { get; set; }

        public bool IsWritten { get; set; }

        private string _readMe;
        private string ReadMe => _readMe ?? (_readMe = ReadMeLine());
        public override string ToString()
        {
            // IssueId
            if (Id == 0) return base.ToString();
            return ReadMe;
        }

        private string ReadMeLine()
        {
            const string separator = "|";
            var builder = new StringBuilder();
            builder.AppendFormat(
                CultureInfo.CurrentCulture,
                "{0} {1}",
                separator,
                Id.ToString("F4", CultureInfo.CurrentCulture.NumberFormat)
                .ToFixedWidth(CorrectionColumnLength.IssueId));
            builder.AppendFormat(
                CultureInfo.CurrentCulture, 
                "{0} {1}",
                separator,
                reportedDate);
            builder.AppendFormat(
                CultureInfo.CurrentCulture, 
                "{0} {1}",
                separator,
                CorrectionDate.ToString("MM-dd-yyyy",
                CultureInfo.CurrentCulture.DateTimeFormat)
                .ToFixedWidth(CorrectionColumnLength.CorrectedDate));
            var commentLength = CorrectionColumnLength.Comments;
            var description = Description.SplitByLength(commentLength, true).ToList();
            if (!description.Any())
            {
                description.Add(string.Empty.ToFixedWidth(commentLength));
            }
            builder.AppendFormat(
                CultureInfo.CurrentCulture, 
                "{0} {1} {0}{2}", 
                separator, 
                description[0], Environment.NewLine);
            if (description.Count > 1)
            {
                const string line = "|           |                |                 |";
                for (int i = 1; i < description.Count; i++)
                {
                    builder.AppendLine($"{line} {description[i].ToFixedWidth(commentLength)} {separator}");
                }
            }
            return builder.ToString();
        }

        public string ToLogEntry(Change change)
        {
            if (change == null) return null;
            var dateReported = change.ReportedDate
                .ToString("MM-dd-yyyy",
                CultureInfo.CurrentCulture.DateTimeFormat)
                .ToFixedWidth(CorrectionColumnLength.ReportedDate);
            var logEntry = ToString().Replace(reportedDate, dateReported);
            return logEntry;
        }
    }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", 
        Justification = "This field needs to be read/write to allow proper serialization.")]
    public class Change
    {
        public string ChangeId { get; set; }

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
