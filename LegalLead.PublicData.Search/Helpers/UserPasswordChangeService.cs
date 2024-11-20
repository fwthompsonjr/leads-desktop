using LegalLead.PublicData.Search.Common;
using Newtonsoft.Json;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Enumerations;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Helpers
{
    public class UserPasswordChangeService : SessionUsagePersistence
    {

        public UserPasswordChangeService(IHttpService service) : base(service)
        {
        }

        public UserPasswordChangedResponse ChangePassword(UserPasswordChangeModel model)
        {
            var changeResponse = new UserPasswordChangedResponse
            {
                Status = false,
                Message = "An error occurred changing password"
            };
            if (string.IsNullOrEmpty(Landing)) return changeResponse;
            var response = GetHttpRespone<UserPasswordChangeModel, PasswordChangedResponse>(model, Landing);
            if (response == null) return changeResponse;
            var mapped = ApiAuthenicationService.GetModel(response.Token, out var _);
            if (mapped != null)
            {
                var json = JsonConvert.SerializeObject(mapped);
                SessionUtil.Write(json);
            }
            changeResponse.Status = true;
            changeResponse.Message = "Password updated successfully.";
            return changeResponse;
        }

        private static string Landing
        {
            get
            {
                if (landing != null) return landing;
                var webid = (int)WebLandingName.Change;
                var service = new CountyCodeService();
                landing = service.GetWebAddress(webid);
                return landing;
            }
        }

        private static string landing = null;

    }
}