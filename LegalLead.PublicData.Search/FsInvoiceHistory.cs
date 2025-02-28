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
            wbViewer.CoreWebView2InitializationCompleted += WbViewer_CoreWebView2InitializationCompleted;

            _ = wbViewer.EnsureCoreWebView2Async().ConfigureAwait(true);
            wbViewer.NavigationStarting += WbViewer_NavigationStarting;
            wbViewer.NavigationCompleted += WbViewer_NavigationCompleted;

            btnPayInvoice.Click += BtnPayInvoice_Click;
        }

        private string webContent;
        private string WebContent
        {
            get => webContent;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    webContent = value;
                }
                else
                {
                    webContent = blankHtmlContent;
                }
            }
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
                        foreach (var kvp in descriptionReplacements)
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
                            Id = h.Id,
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

                var models = list.Select(s => InvoiceHeaderViewModel.ConvertFrom(s)).ToList();
                return models;
            }
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
            public string Id { get; set; } = string.Empty;
            public string CountyName { get; set; }
            public string Description { get; set; }
            public int RecordCount { get; set; }
            public DateTime InvoiceDate { get; set; }
            public string Price { get; set; }
            public string Status { get; set; } = string.Empty;
            public static InvoiceHeaderViewModel ConvertFrom(InvoiceHistoryModel source)
            {
                if (source == null) return new();
                return new()
                {
                    Id = source.Id,
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
            public string Status { get; set; } = string.Empty;
            public string PaymentUrl { get; set; } = string.Empty;
            public string Caption { get; set; } = string.Empty;
            public string Id { get; internal set; }
        }

        private class InvoiceStatusResponse
        {
            public string Id { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
        }

        private static readonly IRemoteInvoiceHelper invoiceReader = ActionSettingContainer
        .GetContainer
        .GetInstance<IRemoteInvoiceHelper>();

        private static readonly SessionUsageReader usageReader = SessionPersistenceContainer
        .GetContainer
        .GetInstance<SessionUsageReader>();


        private readonly List<InvoiceHeaderViewModel> _vwlist;
        private static readonly List<GetUsageResponseContent> rawData = new();
        private static readonly List<InvoiceHistoryModel> masterData = new();
        private static readonly List<InvoiceHtmlModel> htmlData = new();
        private static readonly List<InvoiceHtmlModel> statusData = new();
        private static readonly Dictionary<string, string> descriptionReplacements = new Dictionary<string, string>()
                {
                    { "2024/2020", "2024" },
                    { "2025/2020", "2025" },
                    { "2026/2020", "2026" },
                    { "2027/2020", "2027" },
                    { " from ? to ?/1900", "" },
                };

        private static readonly object sync = new();

        private const string none = "None";
        private const string all_counties = "All counties";
        private const string messageNormal = "Ready";
        private const string messageLoading = "Getting Invoices ...";
        private const string viewNormal = "Return";
        private const string viewInvoice = "View Invoice";

    }
}