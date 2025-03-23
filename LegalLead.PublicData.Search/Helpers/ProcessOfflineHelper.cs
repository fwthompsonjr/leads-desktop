using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using LegalLead.PublicData.Search.Util;

namespace LegalLead.PublicData.Search.Helpers
{
    internal static class ProcessOfflineHelper
    {
        public static ProcessOfflineResponse BeginSearch(ProcessOfflineRequest request)
        {
            var response = dbHelper.BeginSearch(request);
            return response;
        }
        public static ProcessOfflineResponse SearchStatus(ProcessOfflineResponse request)
        {
            var response = dbHelper.GetSearchStatus(request);
            return response;
        }
        private static readonly IRemoteDbHelper dbHelper
            = ActionSettingContainer.GetContainer.GetInstance<IRemoteDbHelper>();
    }
}
