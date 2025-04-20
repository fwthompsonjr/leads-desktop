using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FsOfflineHistory : Form
    {
        private readonly string leadUserId;

        public FsOfflineHistory(string leadId = "")
        {
            InitializeComponent();
            if (!Guid.TryParse(leadId, out var _))
            {
                leadId = UserAccountReader.GetAccountId();
            }
            leadUserId = leadId;
            BindRecords();
            grid.CellContentClick += Grid_CellContentClick;
        }

        private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!grid.Enabled) return;
            try
            {   
                grid.Enabled = false;
                if (e.RowIndex < 0) return;
                if (grid.Columns[e.ColumnIndex].Name != "Download") return;
                if (grid.DataSource is not List<GridHistoryView> accounts) return;
                if (grid.Tag is not string src) return;
                var db = src.ToInstance<List<OfflineStatusResponse>>();
                if (db == null) return;
                if (grid.Rows[e.RowIndex].Tag is GridHistoryView itm && itm.IsComplete) return;
                var item = accounts[e.RowIndex];
                if (!item.IsComplete) return;
                var request = new ProcessOfflineResponse { RequestId = db[e.RowIndex].RequestId };
                var response = ProcessOfflineHelper.DownloadStatus(request);
                if (string.IsNullOrEmpty(response)) return;
                var workitem = response.ToInstance<DownloadPermissionResponse>();
                if (workitem == null || !workitem.Populate()) return;
                var list = workitem.Workload.ToInstance<List<CaseItemDto>>();
                if (list == null) return;
                var obj = new GridHistoryView(new());
                obj.IsComplete = ConvertToWorksheet(request.RequestId, list); // set flag true, if item is downloaded
                grid.Rows[e.RowIndex].Tag = obj;
                if (obj.IsComplete && grid[e.ColumnIndex, e.RowIndex] is DataGridViewButtonCell cell)
                {
                    cell.Value = "Completed";
                }
            }
            finally
            {
                grid.Enabled = true;
            }
        }

        private void BindRecords()
        {
            grid.Columns.Clear();
            var data = ProcessOfflineHelper.GetRequests(leadUserId);
            var view = new List<GridHistoryView>();
            data.ForEach(d => view.Add(new(d)));
            grid.DataSource = null;
            grid.Tag = null;
            grid.Tag = data.ToJsonString();
            grid.DataSource = view;
            grid.ReadOnly = true;
            DataGridViewButtonColumn buttonColumn = new()
            {
                HeaderText = "Download",
                Text = "Download",
                Name = "Download"
            };
            grid.Columns.Add(buttonColumn);
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;            
            grid.Refresh();
        }
        private static bool ConvertToWorksheet(string uniqueId, List<CaseItemDto> list)
        {
            var people = new List<PersonAddress>();
            list.ForEach(l => { people.Add(l.FromDto()); });
            try
            {
                // var fileName = people.GenerateExcelFileName()
                var writer = new ExcelWriter();
                writer.ConvertToPersonTable(people, "Address", websiteId: 60);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private class GridHistoryView(OfflineStatusResponse source)
        {
            public DateTime? StartDate { get; set; } = source.CreateDate;
            public string CountyName { get; set; } = source.CountyName;
            public string CourtType { get; set; } = source.CourtType ?? "-";
            public string DatesSearched { get; set; } = GetDateRange(source);
            public bool IsComplete { get; set; } = source.IsCompleted;
            public decimal PercentComplete { get; set; } = source.PercentComplete.GetValueOrDefault();
            public int RecordCount { get; set; } = source.RecordCount;
            public DateTime? LastUpdate { get; set; } = source.LastUpdate;
        }
        private class DownloadPermissionResponse
        {
            private List<CaseItemDto>? caseItems;
            public List<CaseItemDto> Items => caseItems ?? [];
            public string Id { get; set; }
            public string RequestId { get; set; }
            public string Content { get; set; }
            public string Workload { get; set; }
            public bool CanDownload { get; set; }
            public int? CountyId { get; set; }
            public string? CountyName { get; set; }
            public string? CourtType { get; set; }
            public int? ItemCount { get; set; }
            public bool Populate()
            {
                if (caseItems != null) return caseItems.Any();
                var content = Content;
                var dto = content.ToInstance<DownloadContentDto>();
                if (dto == null) return false;
                Workload = dto.Workload;
                CanDownload = dto.CanDownload;
                CountyId = dto.CountyId.GetValueOrDefault(60);
                CountyName = string.IsNullOrEmpty(dto.CountyName) ? "Dallas" : dto.CountyName;
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
            public string? CountyName { get; set; }
            public string? CourtType { get; set; }
            public int? ItemCount { get; set; }
        }
        private static string GetDateRange(OfflineStatusResponse source)
        {
            if (!source.SearchStartDate.HasValue) return "-";
            var response = $"{source.SearchStartDate.Value:d}";
            if (source.SearchEndDate.HasValue)
            {
                response = $"{response} to {source.SearchEndDate:d}";
            }
            return response;
        }

        private static readonly ISessionPersistance UserAccountReader =
            SessionPersistenceContainer
            .GetContainer
            .GetInstance<ISessionPersistance>(ApiHelper.ApiMode);

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (sender is not System.Windows.Forms.Button btn) return;
            if (!btn.Enabled) return;
            try
            {
                btn.Enabled = false;
                BindRecords();
            }
            finally
            {
                btn.Enabled = true;
            }
        }
    }
}
