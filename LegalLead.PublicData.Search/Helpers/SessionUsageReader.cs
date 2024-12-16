using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
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
                var current = dbhelper.GetHistory(searchDate, false);
                if (current == null || string.IsNullOrEmpty(current.Content)) continue;
                var dataset = current.Content.ToInstance<List<GetUsageResponseContent>>();
                TransformData(fallback, dataset);
            }
            return fallback;
        }

        private static void TransformData(List<UsageHistoryModel> fallback, List<GetUsageResponseContent> imported)
        {
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