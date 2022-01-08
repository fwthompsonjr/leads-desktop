using System;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace LegalLead.Changed.Classes
{
    public class CommandMapChangeIndexes : BuildCommandBase
    {
        public override int Index => 1100;

        public override bool Execute()
        {
            const string allowIt = "true";
            var allMapCorrection = ConfigurationManager
                .AppSettings["Allow.Map.Corrections"] ?? allowIt;
            var allowExec = allMapCorrection.Equals(allowIt,
                StringComparison.CurrentCulture);

            if (!CanExecute())
            {
                return false;
            }

            if (!allowExec)
            {
                return false;
            }

            const int indexInterval = 100;
            var indexId = indexInterval;
            var changeList = Log.Changes
                .Where(a => !string.IsNullOrEmpty(a.ChangeId))
                .Select(c => Convert.ToInt32(c.ChangeId))
                .ToList();

            if (changeList.Count > 0)
            {
                changeList.Sort();
                indexId += changeList.Last();
            }
            var changes = Log.Changes.ToList().FindAll(c => string.IsNullOrEmpty(c.ChangeId));
            if (!changes.Any())
            {
                return true;
            }

            changes.ForEach(c =>
            {
                c.ChangeId = indexId.ToString("0", CultureInfo.CurrentCulture.NumberFormat);
                c.Issues.ToList().ForEach(d => d.ChangeId = c.ChangeId);
                indexId += indexInterval;
            });
            ReSerialize();
            return true;
        }
    }
}
