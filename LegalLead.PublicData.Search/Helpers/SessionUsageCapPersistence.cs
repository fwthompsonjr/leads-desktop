using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Enumerations;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Helpers
{
    public class SessionUsageCapPersistence : SessionUsagePersistence
    {

        public SessionUsageCapPersistence(IHttpService service) : base(service)
        {
        }

        public bool SetUsageLimit(string userId, string county, int recordCount)
        {
            if (string.IsNullOrWhiteSpace(userId) || recordCount < -1) return false;
            if (string.IsNullOrEmpty(Landing)) return false;
            var payload = new { UserName = userId, CountyName = county, MonthlyUsage = recordCount };
            var response = GetHttpRespone<object, object>(payload, Landing);
            return response != null;
        }

        private static string Landing
        {
            get
            {
                if (landing != null) return landing;
                var webid = (int)WebLandingName.UsageCreate;
                var service = new CountyCodeService();
                landing = service.GetWebAddress(webid);
                return landing;
            }
        }

        private static string landing = null;

    }
}