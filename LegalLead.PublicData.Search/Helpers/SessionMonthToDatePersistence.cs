using LegalLead.PublicData.Search.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Helpers
{

    public class SessionMonthToDatePersistence : SessionFilePersistence
    {
        protected SessionUsageReader UsagePersistence
        {
            get
            {
                if (_usagePersistence != null) return _usagePersistence;
                _usagePersistence = SessionPersistenceContainer
                    .GetContainer
                    .GetInstance<SessionUsageReader>();
                return _usagePersistence;
            }
            set { _usagePersistence = value; }
        }

        private SessionUsageReader _usagePersistence = null;
        public void WriteUserRecord()
        {
            var now = DateTime.UtcNow.Date;
            var list = new List<UsageHistoryModel>();
            var data = UsagePersistence.GetUsage(now);
            list.AddRange(data);
            var json = JsonConvert.SerializeObject(list);
            Write(json);
        }

        public int GetCount(string countyName)
        {
            var text = Read();
            if (string.IsNullOrEmpty(text)) return -1;
            var list = text.ToInstance<List<UsageHistoryModel>>();
            if (list == null) return -1;
            var subset = list
                .FindAll(x => x.CountyName.Equals(countyName, StringComparison.OrdinalIgnoreCase));
            return subset.Sum(x => x.MonthlyUsage);
        }

        protected override string SetupFile
        {
            get
            {
                if (setupFileName != null) return setupFileName;
                setupFileName = SessionUtil.GetFullFileName(datFileName);
                return setupFileName;
            }
        }
        private static string setupFileName = null;
        private const string datFileName = "session.mtd";
    }
}