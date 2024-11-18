using System;
using System.Collections.Generic;
using System.Linq;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Helpers
{
    public class SessionApiFilePersistence : SessionFilePersistence
    {
        public int GetUsageLimit(int countyId)
        {
            lock (locker)
            {
                var requested = UsageList().Find(x => x.Id == countyId);
                if (requested == null) return 0;
                var county = requested.CountyName;
                var bo = Read().ToInstance<LeadUserSecurityBo>();
                if (bo == null) return 0;
                var counties = bo.User.CountyData.ToInstance<List<LeadCountyTokenBo>>();
                var selected = counties?.Find(x => x.CountyName.Equals(county, Oic));
                if (selected == null) return 0;
                if (selected.MonthlyLimit != requested.MonthlyLimit)
                {
                    requested.MonthlyLimit = selected.MonthlyLimit;
                }
                if (requested.MonthlyToDate >= requested.MonthlyLimit) return 0;
                return requested.MonthlyLimit;
            }
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
                var userId = bo.User.Id;
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
            var counties = bo.User.CountyData.ToInstance<List<LeadCountyTokenBo>>();
            var selected = counties?.Find(x => x.CountyName.Equals(county, Oic));
            if (selected == null || string.IsNullOrEmpty(selected.Model)) return string.Empty;
            return selected.Model;
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
            var name = Enum.GetName(type);
            name = name.Replace("County", string.Empty);
            name = name.Replace("Civil", string.Empty);
            name = name.Replace("Criminal", string.Empty);
            name = name.Replace("ElPaso", "El Paso");
            name = name.Replace("FortBend", "Fort Bend");
            return name;
        }
        private sealed class LeadCountyTokenBo
        {
            public string LeadUserId { get; set; } = string.Empty;
            public string CountyName { get; set; } = string.Empty;
            public string Model { get; set; } = string.Empty;
            public int MonthlyLimit { get; set; } = 0;
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
