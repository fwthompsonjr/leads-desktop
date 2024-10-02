using System;
using System.Globalization;
using System.Linq;

namespace Harris.Criminal.Db
{
    public static class CaseStyleDbInspector
    {
        private static CultureInfo GetCulture => CultureInfo.InvariantCulture;
        private static StringComparison Oic => StringComparison.OrdinalIgnoreCase;

        public static bool HasHeader(DateTime filingDate)
        {
            var db = Startup.CaseStyles.DataList;
            if (db == null) return false;
            var fileDate = filingDate.ToString("M/d/yyyy", GetCulture);
            return db.Exists(f => f.FileDate.Equals(fileDate, Oic));
        }
        public static bool HasDetail(DateTime filingDate)
        {
            var db = Startup.Downloads.DataList;
            if (!HasHeader(filingDate) || db == null)
            {
                return false;
            }
            var fileDt = filingDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            var fileDate = filingDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            var datasets = db.SelectMany(d => d.Data).ToList();
            var actual = datasets.Find(a => a.FilingDate.Equals(fileDate, Oic) || a.FilingDate.Equals(fileDt, Oic));
            return actual != null;
        }

        public static bool HasDetail(DateTime filingDate, string caseNumber)
        {
            var db = Startup.Downloads.DataList;
            if (!HasDetail(filingDate) || db == null) return false;
            var fileDt = filingDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            var fileDate = filingDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            var datasets = db.SelectMany(d => d.Data).ToList();
            var subset = datasets.FindAll(a => a.FilingDate.Equals(fileDate, Oic) || a.FilingDate.Equals(fileDt, Oic));
            var actual = subset.Find(a => a.CaseNumber.Equals(caseNumber, Oic));
            return actual != null;
        }
    }
}
