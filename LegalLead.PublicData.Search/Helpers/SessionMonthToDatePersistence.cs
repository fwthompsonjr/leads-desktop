using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Documents;
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
            int months = 12;
            var list = new List<UsageHistoryModel>();
            var indexes = Enum.GetValues<SourceType>()
                .Select(s => (int)s).ToList();
            while (months >= 0)
            {
                indexes.ForEach(i =>
                {
                    list.AddRange(UsagePersistence.GetUsage(now.AddMonths(-months)));
                });
                months--;
            }
            var json = JsonConvert.SerializeObject(list);
            Write(json);
        }

        public int GetCount(string countyName)
        {
            var text = Read();
            if (string.IsNullOrEmpty(text)) return -1;

            return 0;
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