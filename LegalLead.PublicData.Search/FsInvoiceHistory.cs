using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using LegalLead.PublicData.Search.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FsInvoiceHistory : Form
    {
        public FsInvoiceHistory()
        {
            InitializeComponent();
            _vwlist = new();
            FormatGrid(dataGridView1);
            btnSubmit.Click += BtnSubmit_Click;
            Shown += FsInvoiceHistory_Shown;
            dataGridView1.RowEnter += DataGridView1_RowEnter;
        }
        private bool AllowExcelRevision { get; set; }
        private void DataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var rowCount = dataGridView1.RowCount;
            lbRecordCount.Text = $"Item {e.RowIndex + 1} of {rowCount}";
        }

        private void FsInvoiceHistory_Shown(object sender, EventArgs e)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += Summary_DoWork;
            worker.RunWorkerCompleted += Summary_RunWorkerCompleted;
            worker.RunWorkerAsync();
            BtnSubmit_Click(null, null);
        }
        /// <summary>
        /// Handles Button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            btnSubmit.Enabled = false;
            dataGridView1.Visible = false;
            toolStrip1.Visible = false;
            lbStatus.Text = "Getting Invoices ...";
            var worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Handles data-binding and user interface state after data completion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is List<InvoiceHeaderViewModel> users)
            {
                _vwlist.Clear();
                _vwlist.AddRange(users);
            }
            dataGridView1.DataSource = _vwlist;
            dataGridView1.Refresh();
            FormatGrid(dataGridView1);
            btnSubmit.Enabled = true;
            dataGridView1.Visible = true;
            lbStatus.Text = "Ready";
            lbRecordCount.Text = $"Item 1 of {_vwlist.Count}";
            TryFormatCounties();
            var summary = new GridSummary
            {
                CountyName = "All counties",
                RecordCount = _vwlist.Count,
                SearchCount = _vwlist.Sum(x => x.RecordCount),
            };
            tslbDataStatus.Text = summary.GetSummary();
            toolStrip1.Visible = true;
        }
        /// <summary>
        /// Fetch data from remote data source on store in IList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = GetHistory();
        }

        /// <summary>
        /// Fetch data from remote data source on store in IList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Summary_DoWork(object sender, DoWorkEventArgs e)
        {
            AllowExcelRevision = false;
            e.Result = GetSummary();
        }

        /// <summary>
        /// Handles data-binding and user interface state after data completion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Summary_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (sync)
            {
                if (e.Result is not List<UsageHistoryModel> models) return;
                usageData.Clear();
                usageData.AddRange(models);
                AllowExcelRevision = true;
            }
        }

        private static void FormatGrid(DataGridView dataGrid)
        {
            Padding padding = Padding.Empty;
            padding.Left = 3;
            padding.Right = 3;
            var columns = dataGrid.Columns;
            if (columns.Count == 0) return;
            dataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            columns[0].Resizable = DataGridViewTriState.False;
            columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            for (var i = 0; i < columns.Count; i++)
            {
                var col = columns[i];
                col.ReadOnly = true;
                col.DefaultCellStyle.Padding = padding;
            }
        }


        private void TryFormatCounties()
        {
            try
            {
                FormatCounties();
            }
            catch (Exception)
            {
                Invoke(() => FormatCounties());
            }
        }
        private void FormatCounties()
        {
            // add options to counties filter,
            // setting no filter as default
            var items = new List<string> { "None" };
            var collection = _vwlist.Select(x => x.CountyName).Distinct().ToList();
            collection.Sort();
            items.AddRange(collection);
            tssbFilterCounty.DropDownItems.Clear();
            items.ForEach(i =>
            {
                var tsOption = new ToolStripMenuItem(i);
                tsOption.Checked = items.IndexOf(i) == 0;
                tsOption.Click += TsOption_Click;
                tssbFilterCounty.DropDownItems.Add(tsOption);
            });
        }

        private void TsOption_Click(object sender, EventArgs e)
        {
            const string none = "None";
            if (sender is not ToolStripMenuItem itm) return;
            if (itm.Checked || itm.Text.Equals(none))
            {
                SelectFilterOption(none);
                dataGridView1.DataSource = _vwlist;
                dataGridView1.Refresh();
                var summary = new GridSummary
                {
                    CountyName = "All counties",
                    RecordCount = _vwlist.Count,
                    SearchCount = _vwlist.Sum(x => x.RecordCount),
                };
                tslbDataStatus.Text = summary.GetSummary();
                return;
            }
            var subset = _vwlist.FindAll(x => x.CountyName == itm.Text);
            dataGridView1.DataSource = subset;
            dataGridView1.Refresh();
            SelectFilterOption(itm.Text);
            var subsummary = new GridSummary
            {
                CountyName = itm.Text,
                RecordCount = subset.Count,
                SearchCount = subset.Sum(x => x.RecordCount),
            };
            tslbDataStatus.Text = subsummary.GetSummary();
            lbRecordCount.Text = $"Item 1 of {subset.Count}";
        }

        private void SelectFilterOption(string selection)
        {
            var items = tssbFilterCounty.DropDownItems;
            foreach (var item in items)
            {
                if (item is ToolStripMenuItem mnu)
                {
                    mnu.Checked = mnu.Text == selection;
                }
            }

        }
        private class GridSummary
        {
            public int RecordCount { get; set; }
            public int SearchCount { get; set; }
            public string CountyName { get; set; }
            public string GetSummary()
            {
                return $"{CountyName} | Found {RecordCount:N0} Entries. {SearchCount:N0} Total Leads.";
            }
        }

        private readonly List<InvoiceHeaderViewModel> _vwlist;
        private static List<InvoiceHeaderViewModel> GetHistory()
        {
            const char space = ' ';
            lock (sync)
            {
                masterData.Clear();                
                var fallback = new List<InvoiceHeaderViewModel>();
                var data = invoiceReader.GetInvoicesByCustomerId();
                if (string.IsNullOrWhiteSpace(data)) return fallback;
                var model = data.ToInstance<GetInvoiceResponse>();
                if (model == null || model.Headers.Count == 0) return fallback;

                var list = new List<InvoiceHistoryModel>();
                var replacements = new Dictionary<string, string>()
                {
                    { "2024/2020", "2024" },
                    { "2025/2020", "2025" },
                    { "2026/2020", "2026" },
                    { "2027/2020", "2027" },
                    { " from ? to ?/1900", "" },
                };
                var countymap = new Dictionary<string, string>()
                {
                    { "El", "El Paso" },
                    { "Fort", "Fort Bend" },
                };
                model.Headers.ForEach(h =>
                {
                    var line = model.Lines.Find(x => x.Id == h.Id && x.LineNbr == 1);
                    if (line != null)
                    {
                        var description = line.Description;
                        foreach(var kvp in replacements)
                        {
                            var find = kvp.Key;
                            var value = kvp.Value;
                            if (description.Contains(find)) description = description.Replace(find, value);
                        }
                        var countyName = description.Split(space)[0];
                        foreach (var kvp in countymap)
                        {
                            var find = kvp.Key;
                            var value = kvp.Value;
                            if (countyName.Equals(find)) countyName = value;
                        }
                        var createDt = h.CreateDate.GetValueOrDefault(DateTime.Now);
                        var price = h.InvoiceTotal.GetValueOrDefault();
                        if (price < 0.50m) price = 0;
                        var addme = new InvoiceHistoryModel
                        {
                            CountyName = countyName,
                            InvoiceDate = createDt,
                            Description = description,
                            RecordCount = h.RecordCount,
                            Price = price.ToString("c2", CultureInfo.CurrentCulture.NumberFormat),
                            Model = h
                        };
                        list.Add(addme);
                    }
                });
                if (list.Count == 0) return fallback;
                list.Sort((a, b) =>
                {
                    var aa = a.CountyName.CompareTo(b.CountyName);
                    if (aa != 0) return aa;
                    return b.InvoiceDate.CompareTo(a.InvoiceDate);
                });
                masterData.AddRange(list);
                var models = list.Select(s => InvoiceHeaderViewModel.ConvertFrom(s));
                return models.ToList();
            }
        }
        private static List<UsageHistoryModel> GetSummary()
        {
            var data = usageReader.GetUsage(DateTime.UtcNow.AddMonths(1));
            var list = new List<UsageHistoryModel>();
            if (data != null && data.Count > 0) { list.AddRange(data); }
            if (list.Count == 0) return list;
            list.Sort((a, b) =>
            {
                var aa = a.CountyName.CompareTo(b.CountyName);
                if (aa != 0) return aa;
                return b.CreateDate.CompareTo(a.CreateDate);
            });
            return list;
        }
        private sealed class InvoiceHeaderViewModel
        {
            public string CountyName { get; set; }
            public string Description { get; set; }
            public int RecordCount { get; set; }
            public DateTime InvoiceDate { get; set; }
            public string Price { get; set; }

            public static InvoiceHeaderViewModel ConvertFrom(InvoiceHistoryModel source)
            {
                if (source == null) return new();
                return new()
                {
                    CountyName = source.CountyName,
                    Description = source.Description,
                    RecordCount = source.RecordCount,
                    InvoiceDate = source.InvoiceDate,
                    Price = source.Price,
                };
            }

        }
        private static readonly List<InvoiceHistoryModel> masterData = new List<InvoiceHistoryModel>();
        private static readonly List<UsageHistoryModel> usageData = new List<UsageHistoryModel>();
        private static readonly object sync = new();
        private static readonly IRemoteInvoiceHelper invoiceReader = ActionSettingContainer
        .GetContainer
        .GetInstance<IRemoteInvoiceHelper>();

        private static readonly SessionUsageReader usageReader = SessionPersistenceContainer
        .GetContainer
        .GetInstance<SessionUsageReader>();
    }
}