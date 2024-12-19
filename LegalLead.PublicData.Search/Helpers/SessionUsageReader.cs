using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Helpers
{
    public class SessionUsageReader : SessionUsagePersistence
    {
        private readonly IRemoteDbHelper dbhelper;
        public SessionUsageReader(IHttpService service, IRemoteDbHelper helper) : base(service)
        {
            dbhelper = helper;
        }

        public virtual List<UsageHistoryModel> GetUsage(DateTime? usageDate = null)
        {
            var fallback = new List<UsageHistoryModel>();
            if (usageDate == null)
            {
                var annual = dbhelper.GetHistory(DateTime.Now, true);
                if (annual == null || string.IsNullOrEmpty(annual.Content)) return fallback;
                var imported = annual.Content.ToInstance<List<GetUsageResponseContent>>();
                TransformData(fallback, imported);
                return fallback;
            }
            // loop backward 12 months
            for (var i = 0; i < 12; i++)
            {
                var searchDate = usageDate.Value.AddMonths(-i);
                var current = dbhelper.GetHistory(searchDate, true);
                if (current == null || string.IsNullOrEmpty(current.Content)) continue;
                var dataset = current.Content.ToInstance<List<GetUsageResponseContent>>();
                TransformData(fallback, dataset);
            }
            return fallback;
        }

        public virtual GetUsageResponseContent GetUsage(int countyId)
        {
            var dnow = DateTime.UtcNow;
            var annual = dbhelper.GetHistory(dnow, true);
            if (annual == null || string.IsNullOrEmpty(annual.Content)) return default;
            var imported = annual.Content.ToInstance<List<GetUsageResponseContent>>();
            if (imported == null || imported.Count == 0) return default;
            imported.RemoveAll(x => !x.CreateDate.HasValue && !x.CompleteDate.HasValue);
            if (imported.Count == 0) return default;
            var usageLimit = GetUsageLimit(countyId);
            var mxRecords = usageLimit?.MaxRecords ?? 15000;
            imported.ForEach(x =>
            {                
                var cdate = x.CreateDate.GetValueOrDefault();
                x.SearchMonth = cdate.Month;
                x.SearchYear = cdate.Year;
                x.MonthlyLimit = mxRecords;
            });
            var subset = imported.FindAll(x => 
                x.CountyId == countyId &&
                x.CompleteDate.HasValue &&
                x.CreateDate.HasValue &&
                x.SearchMonth == dnow.Month &&
                x.SearchYear == dnow.Year);
            if (subset == null || subset.Count == 0) return default;
            var target = subset[0];
            var sum = subset.Sum(x => x.RecordCount);
            target.RecordCount = sum;
            return target;
        }
        public virtual GetMonthlyLimitResponseContent GetUsageLimit(int countyId = 0)
        {
            var response = dbhelper.GetLimits(countyId, false);
            if (response == null || string.IsNullOrEmpty (response.Content)) return null;
            return response.Content.ToInstance<GetMonthlyLimitResponseContent>();
        }



        private static void TransformData(List<UsageHistoryModel> fallback, List<GetUsageResponseContent> imported)
        {
            if (imported == null || imported.Count == 0) return;
            imported.ForEach(x =>
            {
                fallback.Add(new()
                {
                    CountyName = x.CountyName,
                    CreateDate = x.CreateDate.GetValueOrDefault(),
                    DateRange = x.DateRange,
                    IncidentMonth = x.SearchMonth,
                    IncidentYear = x.SearchYear,
                    MonthlyUsage = x.RecordCount
                });
            });
        }

    }
}