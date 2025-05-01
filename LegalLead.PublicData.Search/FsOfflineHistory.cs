using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using LegalLead.PublicData.Search.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

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
            grid.Visible = false;
            lbStatus.Text = "Fetching records. Please wait.";
            grid.CellContentClick += Grid_CellContentClick;
            grid.RowEnter += Grid_RowEnter;
            Shown += FsOfflineHistory_Shown;
        }

        private int DownloadCount { get; set; }

        private void BindRecords()
        {
            grid.Enabled = false;
            var data = ProcessOfflineHelper.GetRequests(leadUserId);
            try
            {
                lbStatus.Text = "Fetching records. Please wait.";
                grid.Columns.Clear();
                grid.Tag = data.ToJsonString();
                var view = new List<GridHistoryView>();
                data.ForEach(d =>
                {
                    var itemId = data.IndexOf(d);
                    var item = new GridHistoryView(d, itemId + 1);
                    item = UpdateMissingFields(itemId, data, item);
                    view.Add(item);
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
                grid.Columns[downLoad].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                var columnRight = new[] { "Id", "PercentComplete", "RecordCount" };
                var columnFull = new[] { "StartDate", "DatesSearched", "LastUpdate" };
                foreach (var column in columnRight) {
                    grid.Columns[column].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                foreach (var column in columnFull)
                {
                    grid.Columns[column].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
                for (int i = 0; i < data.Count; i++)
                {
                    var src = data[i];
                    if (!src.IsCompleted) continue;
                    if (grid.Visible) GenerateContent(i);
                }
                grid.Refresh();
                lbStatus.Text = "Ready";
            }
            finally
            {
                grid.Enabled = true;
            }
        }
        private void GenerateContent(int rowIndex)
        {
            var columnIndex = grid.Columns[downLoad].Index;
            if (grid.DataSource is not List<GridHistoryView> _) return;
            if (grid.Tag is not string src) return;
            var db = src.ToInstance<List<OfflineStatusResponse>>();
            if (db == null) return;
            // clear error-text
            grid.Rows[rowIndex].ErrorText = "";
            if (grid.Rows[rowIndex].Tag is GridHistoryView itm && itm.IsComplete) {
                grid.Rows[rowIndex].ErrorText = "Unable to Download/View. Process is not completed.";
                return; 
            }
            var item = db[rowIndex];
            if (!item.IsCompleted) {
                grid.Rows[rowIndex].ErrorText = "Unable to Download/View. Process is not completed.";
                return; 
            }
            var workitem = GetDownloadDetail(item.RequestId);
            if (workitem == null) {
                grid.Rows[rowIndex].ErrorText = "Unable to Download/View. Record data is not available.";
                return; 
            }
            var list = workitem.Workload.ToInstance<List<CaseItemDto>>();
            if (list == null)
            {
                grid.Rows[rowIndex].ErrorText = "Unable to Download/View. Record data is not available.";
                return;
            }
            var obj = new GridHistoryView(new(), rowIndex);
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
            grid.Rows[rowIndex].Tag = obj;
            if (obj.IsComplete && grid[columnIndex, rowIndex] is DataGridViewButtonCell cell)
            {
                cell.Value = "View";
                DownloadCount++;
                lbProcessed.Text = $"{DownloadCount} Record(s) downloaded.";
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
        private static GridHistoryView UpdateMissingFields(int itemId, List<OfflineStatusResponse> data, GridHistoryView item)
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
                context.RecordCount = list.Count;
                context.FileName = current.FileName;
                return AddItemToMainForm(context, current, grid);
            }
            catch
            {
                return current;
            }
        }

        private static GridHistoryView AddItemToMainForm(GenExcelFileParameter context, GridHistoryView current, DataGridView grid)
        {
            int currentId = current.Id;
            grid.Rows[currentId].ErrorText = "";
            if (!CompleteDbRecord(current, grid))
            {
                grid.Rows[currentId].ErrorText = "Failed to update status. Please retry";
                return current;
            }
            var mainFrm = Program.mainForm;
            if (mainFrm == null) return current;
            var mainTg = mainFrm.Tag.ToJsonString();
            if (string.IsNullOrEmpty(mainTg)) return current;
            var tagged = mainTg.ToInstance<List<SearchResult>>();
            if (tagged == null) return current;
            var member = (SourceType)context.WebsiteId;
            var userName = GetUserName();
            var searchRange = $"{context.StartDate:d} to {context.EndDate:d}";
            var countyName = culture.TextInfo.ToTitleCase(context.CountyName.ToLower());
            var searchDt = context.StartDate.ToShortDateString() + " - " + context.EndDate.ToShortTimeString();
            var courtType = culture.TextInfo.ToTitleCase(context.CourtType.ToLower());
            // update tracking
            var trackingId = context.TrackingIndex;
            if (string.IsNullOrWhiteSpace(userName)) { userName = "unknown"; }
            dbHelper.CompleteUsage(trackingId, context.RecordCount, context.GetShortName());
            var countyNm = member.GetCountyName();
            UsageIncrementer.IncrementUsage(userName, countyNm, context.RecordCount, searchRange);
            UsageReader.WriteUserRecord();
            var dta = new SearchResult
            {
                StartDate = $"{context.StartDate:d}",
                EndDate = $"{context.EndDate:d}",
                Website = $"{countyName} County",
                ResultFileName = current.FileName,
                SearchDate = searchDt,
            };
            dta.Search = $"{DateTime.Now:d} : {dta.Website} ({courtType}) from {dta.StartDate} to {dta.EndDate}";
            tagged.Add(dta);
            mainFrm.Tag = tagged;
            mainFrm.ComboBox_DataSourceChanged(null, null);
            return current;
        }

        private static bool CompleteDbRecord(GridHistoryView current, DataGridView grid)
        {
            if (current == null) return false;
            int currentId = current.Id;
            if (grid.Tag is not string json) return false;
            var db = json.ToInstance<List<OfflineStatusResponse>>();
            if (db == null) return false;
            if (currentId < 0 || currentId > db.Count - 1) return false;
            var item = db[currentId];
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

        private static string GetUserName()
        {
            var container = AuthenicationContainer.GetContainer;
            var userservice = container.GetInstance<SessionUserPersistence>();
            return userservice.GetUserName();
        }

        private static readonly ISessionPersistance UserAccountReader =
            SessionPersistenceContainer
            .GetContainer
            .GetInstance<ISessionPersistance>(ApiHelper.ApiMode);

        private static readonly IRemoteDbHelper dbHelper
            = ActionSettingContainer.GetContainer.GetInstance<IRemoteDbHelper>();

        private static readonly SessionUsagePersistence UsageIncrementer
            = SessionPersistenceContainer.GetContainer
            .GetInstance<SessionUsagePersistence>();
        private static readonly SessionMonthToDatePersistence UsageReader
            = SessionPersistenceContainer.GetContainer
            .GetInstance<SessionMonthToDatePersistence>();

        private const string downLoad = "Download";
        private static readonly CultureInfo culture = new("en-US");
    }
}
