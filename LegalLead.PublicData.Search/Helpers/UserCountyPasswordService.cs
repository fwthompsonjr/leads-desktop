using LegalLead.PublicData.Search.Common;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Enumerations;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Helpers
{
    public class UserCountyPasswordService : SessionUsagePersistence
    {

        public UserCountyPasswordService(IHttpService service) : base(service)
        {
        }

        public bool ChangePassword(UserCountyPasswordModel model)
        {
            if (string.IsNullOrEmpty(Landing)) return false;
            var response = GetHttpRespone<UserCountyPasswordModel, object>(model, Landing);
            return response != null;
        }

        private static string Landing
        {
            get
            {
                if (landing != null) return landing;
                var webid = (int)WebLandingName.County;
                var service = new CountyCodeService();
                landing = service.GetWebAddress(webid);
                return landing;
            }
        }

        private static string landing = null;

    }
}