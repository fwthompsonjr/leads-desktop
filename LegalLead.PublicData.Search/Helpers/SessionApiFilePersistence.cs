using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Helpers
{
    public class SessionApiFilePersistence : SessionFilePersistence
    {
        protected SessionUsagePersistence UsagePersistence
        {
            get
            {
                if (_usagePersistence != null) return _usagePersistence;
                _usagePersistence = SessionPersistenceContainer
                    .GetContainer
                    .GetInstance<SessionUsagePersistence>();
                return _usagePersistence;
            }
            set { _usagePersistence = value; }
        }

        private SessionUsagePersistence _usagePersistence = null;
        public bool IsUsageExceeded(int countyId)
        {
            var requested = UsageList().Find(x => x.Id == countyId);
            if (requested == null) return true;
            var current = GetUsageLimit(countyId);
            if (current == 0) return true;
            if (current == -1) return false;
            return current <= requested.MonthlyToDate;
        }

        public void UpdateUsageCount(int countyId, int recordFound)
        {
            lock (locker)
            {
                var requested = UsageList().Find(x => x.Id == countyId);
                if (requested == null) return;
                requested.MonthlyToDate += recordFound;
                var bo = Read().ToInstance<LeadUserSecurityBo>();
                if (bo == null) return;
                _ = Task.Run(() => { PostUsageIncident(bo.User.Id, requested.CountyName, recordFound); })
                    .ConfigureAwait(false);
            }
        }

        public override string GetAccountPermissions()
        {
            var bo = Read().ToInstance<LeadUserSecurityBo>();
            if (bo == null) return string.Empty;
            var index = bo.User.IndexData.ToInstance<List<LeadIndexesBo>>()?.FirstOrDefault();
            if (index == null || string.IsNullOrEmpty(index.CountyList)) return string.Empty;
            return index.CountyList;
        }
        public override string GetAccountCredential(string county = "")
        {
            var bo = Read().ToInstance<LeadUserSecurityBo>();
            if (bo == null) return string.Empty;
            var counties = bo.User.CountyData.ToInstance<List<LeadCountyTokenModel>>();
            var selected = counties?.Find(x => x.CountyName.Equals(county, Oic));
            if (selected == null || string.IsNullOrEmpty(selected.Model)) return string.Empty;
            return selected.Model;
        }

        public bool UpdateAccountCredential(UserCountyPasswordModel model)
        {
            var county = model.CountyName;
            var token = $"{model.UserName}|{model.Password}";
            var bo = Read().ToInstance<LeadUserSecurityBo>();
            if (bo == null) return false;
            var counties = bo.User.CountyData.ToInstance<List<LeadCountyTokenModel>>();
            var selected = counties?.Find(x => x.CountyName.Equals(county, Oic));
            if (selected == null) return false;
            selected.Model = token;
            bo.User.CountyData = JsonConvert.SerializeObject(counties);
            Write(JsonConvert.SerializeObject(bo));
            return true;
        }

        public int GetUsageLimit(int countyId)
        {
            lock (locker)
            {
                var requested = UsageList().Find(x => x.Id == countyId);
                if (requested == null) return 0;
                var county = requested.CountyName;
                var bo = Read().ToInstance<LeadUserSecurityBo>();
                if (bo == null) return 0;
                var counties = bo.User.CountyData.ToInstance<List<LeadCountyTokenModel>>();
                var selected = counties?.Find(x => x.CountyName.Equals(county, Oic));
                if (selected == null)
                {
                    requested.MonthlyLimit = 0;
                    return 0;
                }
                var selectedMonthlyLimit = selected.MonthlyLimit.GetValueOrDefault(0);
                if (selectedMonthlyLimit == -1) return -1;
                if (selected.MonthlyLimit != requested.MonthlyLimit)
                {
                    requested.MonthlyLimit = selectedMonthlyLimit;
                }
                if (requested.MonthlyToDate >= requested.MonthlyLimit) return 0;
                return requested.MonthlyLimit;
            }
        }

        private void PostUsageIncident(string id, string countyName, int recordFound)
        {
            try
            {
                UsagePersistence.IncrementUsage(id, countyName, recordFound);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
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
        private static List<CountyIndexBo> UsageList()
        {
            lock (locker)
            {
                if (usageList != null) return usageList;
                usageList = Enum.GetValues<SourceType>()
                    .Select(s => new CountyIndexBo
                    {
                        Id = (int)s,
                        CountyName = GetSourceTypeName(s)
                    }).ToList();
                return usageList;
            }
        }
        private static string GetSourceTypeName(SourceType type)
        {
            var name = type.GetCountyName();
            return name;
        }

        private sealed class LeadIndexesBo
        {
            public string LeadUserId { get; set; } = string.Empty;
            public string CountyList { get; set; } = string.Empty;
        }
        private sealed class CountyIndexBo
        {
            public int Id { get; set; } = -1;
            public string CountyName { get; set; } = string.Empty;
            public int MonthlyLimit { get; set; } = 0;
            public int MonthlyToDate { get; set; } = 0;
        }
        private static List<CountyIndexBo> usageList = null;
        private static string setupFileName = null;
        private const string datFileName = "session.dtx";
        private static readonly StringComparison Oic = StringComparison.OrdinalIgnoreCase;
        private static readonly object locker = new();
    }
}
