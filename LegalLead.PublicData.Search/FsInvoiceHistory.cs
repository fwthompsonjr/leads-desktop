using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using LegalLead.PublicData.Search.Util;
using System;
using System.Collections.Generic;
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
            AllowExcelRevision = false;
            AllowPreviewInvoice = false;
            AllowDataRefresh = false;
            _vwlist = new();
            FormatGrid(dataGridView1);
            btnSubmit.Click += BtnSubmit_Click;
            Shown += FsInvoiceHistory_Shown;
            dataGridView1.RowEnter += DataGridView1_RowEnter;
        }
        private DisplayModes CurrentDisplay { get; set; } = DisplayModes.None;

        private bool AllowExcelRevision { get; set; }
        private bool AllowPreviewInvoice { get; set; }
        private bool AllowDataRefresh { get; set; }

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
                        foreach (var kvp in replacements)
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

        private static List<GetUsageResponseContent> GetRawData()
        {
            var data = usageReader.GetUsageRawData(DateTime.UtcNow.AddMonths(1));
            var list = new List<GetUsageResponseContent>();
            if (data != null && data.Count > 0) { list.AddRange(data); }
            if (list.Count == 0) return list;
            list.Sort((a, b) =>
            {
                var aa = a.CountyName.CompareTo(b.CountyName);
                if (aa != 0) return aa;
                return b.CreateDate.GetValueOrDefault().CompareTo(a.CreateDate.GetValueOrDefault());
            });
            return list;
        }

        private sealed class GridSummary
        {
            public int RecordCount { get; set; }
            public int SearchCount { get; set; }
            public string CountyName { get; set; }
            public string GetCaptionText()
            {
                return $"{CountyName} | Found {RecordCount:N0} Entries. {SearchCount:N0} Total Leads.";
            }
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
        
        private sealed class InvoiceHtmlModel
        {
            public string CountyName { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int RecordCount { get; set; } 
            public DateTime InvoiceDate { get; set; } 
            public string Price { get; set; } = string.Empty;
            public string Html { get; set; } = string.Empty;

        }

        private static readonly IRemoteInvoiceHelper invoiceReader = ActionSettingContainer
        .GetContainer
        .GetInstance<IRemoteInvoiceHelper>();

        private static readonly SessionUsageReader usageReader = SessionPersistenceContainer
        .GetContainer
        .GetInstance<SessionUsageReader>();


        private readonly List<InvoiceHeaderViewModel> _vwlist;
        private static readonly List<InvoiceHistoryModel> masterData = new();
        private static readonly List<UsageHistoryModel> usageData = new();
        private static readonly List<GetUsageResponseContent> rawData = new();
        private static readonly List<InvoiceHtmlModel> htmlData = new();

        private static readonly object sync = new();

        private const string none = "None";
        private const string all_counties = "All counties";
        private const string messageNormal = "Ready";
        private const string messageLoading = "Getting Invoices ...";
    }
}