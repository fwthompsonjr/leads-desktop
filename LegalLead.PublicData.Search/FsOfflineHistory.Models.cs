using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("SQ Warning",
        "S3459:Unassigned members should be removed",
        Justification = "unassigned fields needs to deserialize json responses")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("SQ Warning",
        "S1144:Unassigned set accessor",
        Justification = "fields needed to deserialize json responses")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("SQ Warning",
        "S3878:simplify collection of elements",
        Justification = "")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Warning",
        "CA1303:Put localized strings in resource table",
        Justification = "tech debit to be addressed")]
    public partial class FsOfflineHistory
    {
        private class DownloadFlagResponse
        {
            public string RequestId { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
        }

        private class DownloadFlagStatus
        {
            public string RequestId { get; set; } = string.Empty;

            public bool IsCompleted { get; set; }
        }
        private class GridHistoryView(OfflineStatusResponse source, int index = 0)
        {
            public int Id { get { return index; } }
            public DateTime? StartDate { get; set; } = source.CreateDate;
            public string CountyName { get; set; } = source.CountyName;
            public string CourtType { get; set; } = source.CourtType ?? "-";
            public string DatesSearched { get; set; } = GetDateRange(source);
            public bool IsComplete { get; set; } = source.IsCompleted;
            public decimal PercentComplete { get; set; } = source.PercentComplete.GetValueOrDefault();
            public int RecordCount { get; set; } = source.RecordCount;
            public DateTime? LastUpdate { get; set; } = source.LastUpdate;
            public string FileName { get; set; } = string.Empty;
            public bool NewNameCompleted { get; set; } = false;
        }
        private class DownloadPermissionResponse
        {
            private List<CaseItemDto> caseItems;
            public string Id { get; set; }
            public string RequestId { get; set; }
            public string Content { get; set; }
            public string Workload { get; set; }
            public bool CanDownload { get; set; }
            public int? CountyId { get; set; }
            public string CountyName { get; set; }
            public string CourtType { get; set; }
            public int? ItemCount { get; set; }
            public bool Populate()
            {
                if (caseItems != null) return caseItems.Count != 0;
                var content = Content;
                var dto = content.ToInstance<DownloadContentDto>();
                if (dto == null) return false;
                Workload = dto.Workload;
                CanDownload = dto.CanDownload;
                CountyId = dto.CountyId.GetValueOrDefault(60);
                CountyName = (string.IsNullOrEmpty(dto.CountyName) ? "Dallas" : dto.CountyName).ToUpper();
                CourtType = string.IsNullOrEmpty(dto.CourtType) ? "COUNTY" : dto.CourtType;
                ItemCount = dto.ItemCount;
                Id = dto.Id;
                var items = dto.Workload.ToInstance<List<CaseItemDto>>();
                if (!dto.CanDownload || items == null)
                {
                    caseItems = [];
                    return false;
                }
                caseItems = items ?? [];
                if (caseItems.Count > 0)
                {
                    var caseNumber = items.Find(x => !string.IsNullOrEmpty(x.CaseNumber))?.CaseNumber;
                    if (caseNumber == null || !caseNumber.Contains('-')) return true;
                    var caseIndex = caseNumber.Split('-')[0];
                    CourtType = caseIndex switch
                    {
                        "CC" => "COUNTY",
                        "JPC" => "JUSTICE",
                        "DC" => "DISTRICT",
                        _ => CourtType,
                    };
                }
                return true;
            }

        }

        private class DownloadContentDto
        {
            public string Id { get; set; }
            public string RequestId { get; set; }
            public string Workload { get; set; }
            public bool CanDownload { get; set; }
            public int? CountyId { get; set; }
            public string CountyName { get; set; }
            public string CourtType { get; set; }
            public int? ItemCount { get; set; }
        }
    }
}
