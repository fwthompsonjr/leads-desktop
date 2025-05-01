using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using LegalLead.PublicData.Search.Util;
using LegalLead.PublicData.Search.Extensions;
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

        public static string DownloadStatus(ProcessOfflineResponse request)
        {
            var response = dbHelper.GetDownloadStatus(request);
            return response;
        }

        public static string FlagDownloadCompleted(object request)
        {
            var response = dbHelper.FlagDownloadCompleted(request);
            return response;
        }
        public static List<OfflineStatusResponse> GetRequests(string leadId)
        {
            var response = dbHelper.GetOfflineRequests(new() { LeadId = leadId });
            if (response == null || response.Count == 0) return response;
            // convert dates to CST
            response.ForEach(r =>
            {
                if (r.CreateDate.HasValue) r.CreateDate = r.CreateDate.Value.ToCentralTime();
                if (r.LastUpdate.HasValue) r.LastUpdate = r.LastUpdate.Value.ToCentralTime();
            });
            var details = GetRequestSearchDetails(leadId);
            if (details == null || details.Count == 0) return response;
            response.ForEach(r =>
            {
                var src = details.Find(x => (x.Id ?? "").Equals(r.OfflineId));
                if (src != null && string.IsNullOrEmpty(r.CourtType)) { 
                    r.CourtType = src.SearchType;
                }
                if (src != null && r.RecordCount == 0)
                {
                    r.RecordCount = src.ItemCount.GetValueOrDefault();
                }
            });
            return response;
        }

        public static List<OfflineSearchTypeResponse> GetRequestSearchDetails(string leadId)
        {
            var response = dbHelper.GetRequestSearchDetails(leadId);
            return response;
        }

        private static readonly IRemoteDbHelper dbHelper
            = ActionSettingContainer.GetContainer.GetInstance<IRemoteDbHelper>();
    }
}
