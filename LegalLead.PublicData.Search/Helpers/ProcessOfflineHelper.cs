using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using LegalLead.PublicData.Search.Util;
using System.Collections.Generic;

namespace LegalLead.PublicData.Search.Helpers
{
    internal static class ProcessOfflineHelper
    {
        public static ProcessOfflineResponse BeginSearch(ProcessOfflineRequest request, string subContext = "")
        {
            var items = new List<string> { "COUNTY", "DISTRICT", "JUSTICE" };
            var response = dbHelper.BeginSearch(request);
            if (items.Contains(subContext))
            {
                dbHelper.UpdateSearchContext(request, subContext);
            }
            return response;
        }
        public static ProcessOfflineResponse SearchStatus(ProcessOfflineResponse request)
        {
            var response = dbHelper.GetSearchStatus(request);
            return response;
        }
        /*
        need ability to get download status
        */

        public static string DownloadStatus(ProcessOfflineResponse request)
        {
            var response = dbHelper.GetDownloadStatus(request);
            return response;
        }

        public static List<OfflineStatusResponse> GetRequests(string leadId)
        {
            var response = dbHelper.GetOfflineRequests(new() { LeadId = leadId });
            return response;
        }
        private static readonly IRemoteDbHelper dbHelper
            = ActionSettingContainer.GetContainer.GetInstance<IRemoteDbHelper>();
    }
}
