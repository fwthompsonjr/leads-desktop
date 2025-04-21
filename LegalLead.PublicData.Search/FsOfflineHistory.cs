using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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
            grid.ReadOnly = true;
            BindRecords();
            grid.CellContentClick += Grid_CellContentClick;
        }

        private void BindRecords()
        {
            grid.Enabled = false;
            var data = ProcessOfflineHelper.GetRequests(leadUserId);
            try
            {

                grid.Columns.Clear();
                
                var view = new List<GridHistoryView>();
                data.ForEach(d =>
                {
                    var itemId = data.IndexOf(d);
                    var item = new GridHistoryView(d, itemId);
                    view.Add(item);

                    _ = Task.Run(() =>
                    {
                        item = UpdateMissingFields(itemId, data, item);
                    });
                });
                grid.DataSource = null;
                grid.DataSource = view;
                DataGridViewButtonColumn buttonColumn = new()
                {
                    HeaderText = downLoad,
                    Text = downLoad,
                    Name = downLoad
                };
                grid.Columns.Add(buttonColumn);
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                grid.Columns["FileName"].Visible = false;
                grid.Columns["NewNameCompleted"].Visible = false;
                grid.Refresh();
                var buttonId = grid.Columns[downLoad].Index;
                for (int i = 0; i < data.Count; i++) {
                    var cell = grid[buttonId, i];
                    if (cell is not DataGridViewButtonCell btnCell) continue;
                    btnCell.Value = downLoad;
                }
                grid.Refresh();
            }
            finally
            {
                grid.Tag = data.ToJsonString();
                grid.Enabled = true;
            }
        }

        private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!grid.Enabled) return;
            try
            {   
                grid.Enabled = false;
                if (e.RowIndex < 0) return;
                if (grid.Columns[e.ColumnIndex].Name != downLoad) return;
                if (CanOpenFile(e.RowIndex, e.ColumnIndex))
                {
                    OpenFile(e.RowIndex);
                    return;
                }
                if (grid.DataSource is not List<GridHistoryView> accounts) return;
                if (grid.Tag is not string src) return;
                var db = src.ToInstance<List<OfflineStatusResponse>>();
                if (db == null) return;
                if (grid.Rows[e.RowIndex].Tag is GridHistoryView itm && itm.IsComplete) return;
                var item = db[e.RowIndex];
                if (!item.IsCompleted) return;
                var workitem = GetDownloadDetail(item.RequestId);
                if (workitem == null) return;
                var list = workitem.Workload.ToInstance<List<CaseItemDto>>();
                if (list == null) return;
                var obj = new GridHistoryView(new(), e.RowIndex);
                var context = new GenExcelFileParameter
                {
                    WebsiteId = workitem.CountyId.GetValueOrDefault(60),
                    CountyName = workitem.CountyName,
                    CourtType = workitem.CourtType,
                    TrackingIndex = workitem.RequestId,
                    StartDate = item.SearchStartDate.GetValueOrDefault(),
                    EndDate = item.SearchEndDate.GetValueOrDefault(),
                };
                obj = ConvertToWorksheet(context, list, obj, grid); // set flag true, if item is downloaded
                grid.Rows[e.RowIndex].Tag = obj;
                if (obj.IsComplete && grid[e.ColumnIndex, e.RowIndex] is DataGridViewButtonCell cell)
                {
                    cell.Value = "View";
                }
            }
            finally
            {
                grid.Enabled = true;
            }
        }

        private bool CanOpenFile(int rowIndex, int downloadColumnIndex)
        {
            if (grid.Enabled) return false;
            try
            {
                var cell = grid[downloadColumnIndex, rowIndex];
                if (cell is not DataGridViewButtonCell gridButton) return false;
                if (gridButton.Value is not string buttonText) return false;
                if (!buttonText.Equals("View")) return false;
                if (grid.Rows[rowIndex].Tag is not GridHistoryView itm || !itm.IsComplete) return false;
                if (string.IsNullOrWhiteSpace(itm.FileName) || !System.IO.File.Exists(itm.FileName)) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void OpenFile(int rowIndex)
        {
            var row = grid.Rows[rowIndex];
            if (row.Tag is not GridHistoryView itm || !itm.IsComplete) return;
            if (string.IsNullOrWhiteSpace(itm.FileName) || !System.IO.File.Exists(itm.FileName)) return;
            if (!itm.NewNameCompleted)
            {
                var newFileName = CommonFolderHelper.MoveToCommon(itm.FileName);
                if (System.IO.File.Exists(newFileName))
                {
                    itm.NewNameCompleted = true;
                    itm.FileName = newFileName;
                    row.Tag = itm;
                }
            }
            using var p = new Process();
            p.StartInfo = new ProcessStartInfo(itm.FileName)
            {
                UseShellExecute = true
            };
            p.Start();
        }
        private static DownloadPermissionResponse GetDownloadDetail(string requestId)
        {
            var request = new ProcessOfflineResponse { RequestId = requestId };
            var response = ProcessOfflineHelper.DownloadStatus(request);
            if (string.IsNullOrEmpty(response)) return default;
            var workitem = response.ToInstance<DownloadPermissionResponse>();
            if (workitem == null || !workitem.Populate()) return default;
            return workitem;
        }
        private GridHistoryView UpdateMissingFields(int itemId, List<OfflineStatusResponse> data, GridHistoryView item)
        {
            var source = data[itemId];
            var detail = GetDownloadDetail(source.RequestId);
            if (detail == null) return item;
            item.RecordCount = detail.ItemCount.GetValueOrDefault();
            item.CourtType = detail.CourtType;
            return item;
        }

        private static GridHistoryView ConvertToWorksheet(GenExcelFileParameter context, List<CaseItemDto> list, GridHistoryView current, DataGridView grid)
        {
            var people = new List<PersonAddress>();
            list.ForEach(l => { people.Add(l.FromDto()); });
            try
            {
                current.IsComplete = false;
                current.FileName = string.Empty;
                var fileName = people.GenerateExcelFileName(context);
                var generatedSuccess = System.IO.File.Exists(fileName);
                if (current != null)
                {
                    current.FileName = fileName;
                    current.IsComplete = generatedSuccess;
                }
                if (!generatedSuccess) return current;
                var nwName = CommonFolderHelper.MoveToCommon(current.FileName);
                if (System.IO.File.Exists(nwName))
                {
                    current.FileName = nwName;
                    current.NewNameCompleted = true;
                }
                return AddItemToMainForm(context, current, grid);
            }
            catch
            {
                return current;
            }
        }

        private static GridHistoryView AddItemToMainForm(GenExcelFileParameter context, GridHistoryView current, DataGridView grid)
        {
            grid.Rows[current.Id].ErrorText = "";
            if (!CompleteDbRecord(current, grid)) 
            {
                grid.Rows[current.Id].ErrorText = "Failed to update status. Please retry";
                return current; 
            }
            /*
             * NOTE: need to push the filename to remote with tracking id.
             * this ensures that when invoice is paid that user can see their records if not an admin.
            */
            var mainFrm = Program.mainForm;
            if (mainFrm == null) return current;
            var mainTg = mainFrm.Tag.ToJsonString();
            if (string.IsNullOrEmpty(mainTg)) return current;
            var tagged = mainTg.ToInstance<List<SearchResult>>();
            if (tagged == null) return current;
            var countyName = culture.TextInfo.ToTitleCase(context.CountyName.ToLower());
            var searchDt = context.StartDate.ToShortDateString() + " - " + context.EndDate.ToShortTimeString(); 
            var courtType = culture.TextInfo.ToTitleCase(context.CourtType.ToLower());
            var dta = new SearchResult
            {
                StartDate = $"{context.StartDate:d}",
                EndDate = $"{context.EndDate:d}",
                Website = $"{countyName} County",
                ResultFileName = current.FileName,
                SearchDate = searchDt,
            };
            dta.Search = $"{dta.SearchDate} : {dta.Website} ({courtType}) from {dta.StartDate} to {dta.EndDate}";
            tagged.Add(dta);
            mainFrm.Tag = tagged;
            mainFrm.ComboBox_DataSourceChanged(null, null);
            return current;
        }

        private static bool CompleteDbRecord(GridHistoryView current, DataGridView grid)
        {
            if (current == null) return false;
            if (grid.Tag is not string json) return false;
            var db = json.ToInstance<List<OfflineStatusResponse>>();
            if (db == null) return false;
            if (current.Id < 0 || current.Id > db.Count - 1) return false;
            var item = db[current.Id];
            var dte = $"{DateTime.UtcNow:G}";
            var payload = new
            {
                item.OfflineId,
                item.RequestId,
                CanDownload = item.IsCompleted,
                Message = string.Join(Environment.NewLine, new[]
                {
                    $"{dte} | User completed download from user interface",
                    $"{dte} | County {current.CountyName}, {current.CourtType}",
                    $"{dte} | Search Range {current.DatesSearched}",
                    $"{dte} | Record Count {current.RecordCount}",
                })
            };
            var updated = ProcessOfflineHelper.FlagDownloadCompleted(payload);
            if (string.IsNullOrEmpty(updated)) return false;
            var flag = updated.ToInstance<DownloadFlagResponse>();
            if (flag == null || string.IsNullOrWhiteSpace(flag.Content)) return false;
            var sts = flag.Content.ToInstance<DownloadFlagStatus>();
            if (sts == null || !sts.IsCompleted) return false;
            return true;
        }
        private class DownloadFlagResponse
        {
            public string RequestId { get; set; }
            public string Content { get; set; }
        }

        private class DownloadFlagStatus
        {
            public string RequestId { get; set; }
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

        private const string downLoad = "Download";
        private static CultureInfo culture = new CultureInfo("en-US"); 
    }
}
