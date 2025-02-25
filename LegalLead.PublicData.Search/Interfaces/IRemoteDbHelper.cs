using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface IRemoteDbHelper
    {
        AppendUsageRecordResponse AppendUsage(int countyId, DateTime startDate, DateTime endDate);
        FindDbResponse Begin(FindDbRequest findDb);
        FindDbResponse Complete(FindDbRequest findDb);
        CompleteUsageRecordResponse CompleteUsage(string recordId, int recordCount, string excelName = "");
        GetUsageResponse GetHistory(DateTime searchDate, bool getAllCounties);
        GetMonthlyLimitResponse GetLimits(int countyId, bool getAllCounties);
        GetUsageResponse GetSummary(DateTime searchDate, bool getAllCounties);
        List<HolidayQueryResponse> Holidays();
        List<QueryDbResponse> Query(QueryDbRequest queryDb);
        KeyValuePair<bool, string> Upload(UploadDbRequest uploadDb);
        void PostFileDetail(SearchContext context);
    }
}
