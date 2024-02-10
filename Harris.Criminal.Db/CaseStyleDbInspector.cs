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
            var fileDate = filingDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            foreach (var dataset in db)
            {
                var found = dataset.Data.Exists(a => a.FilingDate.Equals(fileDate, Oic));
                if (!found)
                {
                    continue;
                }

                found = dataset.Data.Exists(a =>
                    a.FilingDate.Equals(fileDate, Oic));
                if (found)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasDetail(DateTime filingDate, string caseNumber)
        {
            var db = Startup.Downloads.DataList;
            if (!HasDetail(filingDate) || db == null)
            {
                return false;
            }
            var fileDate = filingDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            foreach (var dataset in db)
            {
                var found = dataset.Data.Exists(a => a.FilingDate.Equals(fileDate, Oic));
                if (!found)
                {
                    continue;
                }

                found = dataset.Data.Exists(a =>
                    a.FilingDate.Equals(fileDate, Oic) &&
                    a.CaseNumber.Equals(caseNumber, Oic));
                if (found)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
