using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Enumerations;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Helpers
{
    public class SessionUsageReader : SessionUsagePersistence
    {

        public SessionUsageReader(IHttpService service) : base(service)
        {
        }

        public virtual List<UsageHistoryModel> GetUsage(DateTime? usageDate = null)
        {
            var fallback = new List<UsageHistoryModel>();
            if (string.IsNullOrEmpty(Landing)) return fallback;
            usageDate ??= DateTime.UtcNow;
            var dte = usageDate.Value.ToString("s").Split('T')[0];
            var payload = new { CreateDate = dte };
            var response = GetHttpRespone<object, List<UsageHistoryModel>>(payload, Landing);
            return response ?? fallback;
        }
        private static string Landing
        {
            get
            {
                if (landing != null) return landing;
                var webid = (int)WebLandingName.UsageList;
                var service = new CountyCodeService();
                landing = service.GetWebAddress(webid);
                return landing;
            }
        }

        private static string landing = null;

    }
}