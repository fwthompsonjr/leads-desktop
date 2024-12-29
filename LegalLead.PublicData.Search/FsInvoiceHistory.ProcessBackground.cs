using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search
{
    using AG = HtmlAgilityPack;
    public partial class FsInvoiceHistory : Form
    {
        /// <summary>
        /// Verify excel file status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExcelData_DoWork(object sender, DoWorkEventArgs e)
        {
            rawData.RemoveAll(x => string.IsNullOrWhiteSpace(x.ExcelName));
            rawData.RemoveAll(x => x.RecordCount == 0);
            var indexes = rawData.Select(s =>
            {
                var src = _vwlist.Find(x => x.Id == s.Id);
                return new UsageExcelIndex
                {
                    Id = s.Id,
                    LeadUserId = s.LeadUserId,
                    RecordCount = s.RecordCount,
                    ExcelName = s.ExcelName,
                    Status = src?.Status ?? "UNKNOWN"
                };
            }).ToList();
            if (indexes.Count == 0) return;
            indexes = indexes.FindAll(x => x.RecordCount > 0 && !string.IsNullOrEmpty(x.ExcelName));
            if (indexes.Count == 0) return;
            new ExcelFileUnlockService(invoiceReader, indexes).Process();
        }

        /// <summary>
        /// Fetch data from remote data source on store in IList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MasterData_DoWork(object sender, DoWorkEventArgs e)
        {
            if (AllowPreviewInvoice)
            {
                // only going to fetch invoice content once in lifetime of form
                e.Result = null;
                return;
            }
            // get invoice html from service
            var target = new List<InvoiceHtmlModel>();
            var list = new List<InvoiceHistoryModel>();
            list.AddRange(masterData);
            Parallel.ForEach(list,
                new ParallelOptions() { MaxDegreeOfParallelism = 5 },
                m =>
                {
                    var invoiceData = m.Model.ToJsonString();
                    var html = invoiceReader.PreviewInvoice(invoiceData);
                    html = RemovePaymentButton(html);
                    html = StandarizeDescriptions(html);
                    var item = new InvoiceHtmlModel
                    {
                        Id = m.Model.Id,
                        CountyName = m.CountyName,
                        Description = m.Description,
                        RecordCount = m.RecordCount,
                        InvoiceDate = m.InvoiceDate,
                        Price = m.Price,
                        Html = html
                    };
                    if (!string.IsNullOrEmpty(html))
                    {
                        AppendHtmlAttributes(item);
                    }
                    target.Add(item);
                });
            var items = target.FindAll(x => !x.Status.Equals("PAID"));
            AppendStatusCheck(items);

            e.Result = target;
        }

        private static void AppendStatusCheck(InvoiceHtmlModel item)
        {
            lock (sync)
            {
                if (!statusData.Exists(x => x.Id == item.Id)) statusData.Add(item);
            }
        }

        private static void AppendStatusCheck(List<InvoiceHtmlModel> items)
        {
            lock (sync)
            {
                items.ForEach(i =>
                {
                    if (!statusData.Exists(x => x.Id == i.Id)) statusData.Add(i);
                }); 
            }
        }

        /// <summary>
        /// Handles data-binding and user interface state after data completion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MasterData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (sync)
            {
                if (e.Result is not List<InvoiceHtmlModel> models) return;
                htmlData.Clear();
                htmlData.AddRange(models);
                AllowPreviewInvoice = true;
                AllowDataRefresh = true;
                SetDisplay(DisplayModes.Normal);
            }
        }
        /// <summary>
        /// Fetch data from remote data source on store in IList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RawData_DoWork(object sender, DoWorkEventArgs e)
        {
            if (AllowExcelRevision)
            {
                e.Result = null;
                return;
            }
            e.Result = GetRawData();
        }

        /// <summary>
        /// Handles data-binding and user interface state after data completion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RawData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                lock (sync)
                {
                    if (e.Result is not List<GetUsageResponseContent> models) return;
                    rawData.Clear();
                    rawData.AddRange(models);
                    AllowExcelRevision = true;
                }
            }
            finally
            {
                var worker = new BackgroundWorker();
                worker.DoWork += ExcelData_DoWork;
                worker.RunWorkerAsync();
            }
        }


        /// <summary>
        /// Fetch data from remote data source on store in IList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusData_DoWork(object sender, DoWorkEventArgs e)
        {
            var list = new List<InvoiceHistoryModel>();
            var statusList = new List<InvoiceStatusResponse>();
            list.AddRange(masterData);
            Parallel.ForEach(list,
                new ParallelOptions() { MaxDegreeOfParallelism = 5 },
                m =>
                {
                    var js = m.Model.ToJsonString();
                    var dta = invoiceReader.GetInvoiceStatus(js);
                    var converted = dta.ToInstance<InvoiceStatusResponse>();
                    if (converted != null) statusList.Add(converted);
                });
            e.Result = statusList;
        }

        /// <summary>
        /// Handles data-binding and user interface state after data completion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (sync)
            {
                if (e.Result is not List<InvoiceStatusResponse> statuses) return;
                statuses.ForEach(s =>
                {
                    var item = _vwlist.Find(v => v.Id == s.Id);
                    if (item != null) item.Status = s.Status;
                });
            }
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
        /// Handles data-binding and user interface state after data completion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Result is List<InvoiceHeaderViewModel> users)
                {
                    _vwlist.Clear();
                    _vwlist.AddRange(users);
                }
                dataGridView1.DataSource = _vwlist;
                dataGridView1.Refresh();
                FormatGrid(dataGridView1);
                FormatCounties();
                SetDataCaption(all_counties, _vwlist);
                var count = _vwlist.Count;
                lbRecordCount.Text = GetRowCountLabel(1, count);
                SetDisplay(DisplayModes.Normal);
            }
            finally
            {
                var bw = new BackgroundWorker();
                bw.DoWork += MasterData_DoWork;
                bw.RunWorkerCompleted += MasterData_RunWorkerCompleted;
                bw.RunWorkerAsync();

                var bw2 = new BackgroundWorker();
                bw2.DoWork += StatusData_DoWork;
                bw2.RunWorkerCompleted += StatusData_RunWorkerCompleted;
                bw2.RunWorkerAsync();
            }
        }



        private static void AppendHtmlAttributes(InvoiceHtmlModel item)
        {
            var selectors = new[]
            {
                "//span[@name = 'invoice-status']",
                "//*[@id = 'spn-payment-address']"
            }.ToList();
            var doc = new AG.HtmlDocument();
            doc.LoadHtml(item.Html);
            var docNode = doc.DocumentNode;
            selectors.ForEach(selector =>
            {
                var indx = selectors.IndexOf(selector);
                var sourceNode = docNode.SelectSingleNode(selector);
                if (sourceNode != null)
                {
                    var txt = sourceNode.InnerText.Trim();
                    AppendHtmlAttributes(item, indx, txt);
                }
            });
            var titleItems = new List<string>
            {
                "//h5[@class = 'card-title text-start']",
                "//span[@name = 'invoice-status']",
                "//span[@name = 'invoice-total']",
            };
            var titles = new List<string>();
            foreach (var title in titleItems)
            {
                var sourceNode = docNode.SelectSingleNode(title);
                if (sourceNode == null) { titles.Add(" - "); continue; }
                var txt = sourceNode.InnerText
                    .Replace("Invoice:", "")
                    .Replace("&nbsp;", "")
                    .Replace("nbsp;", "").Trim();
                var data = string.IsNullOrWhiteSpace(txt) ? " - " : txt;
                titles.Add(data);
            }
            if (titles[1].Equals("PAID") &&
                titles[2].Equals("$0.01") &&
                titles[0].Equals(" - "))
            {
                item.Caption = "PAID | Non billable search";
                return;
            }
            item.Caption = string.Join(" | ", titles);
        }

        private static void AppendHtmlAttributes(InvoiceHtmlModel item, int fieldId, string fieldValue)
        {
            const string paymentToken = "stripe.com";
            if (string.IsNullOrWhiteSpace(fieldValue)) return;
            if (fieldId == 0)
            {
                item.Status = fieldValue;
                return;
            }
            if (!fieldValue.Contains(paymentToken, StringComparison.OrdinalIgnoreCase)) return;
            item.PaymentUrl = fieldValue;
        }

        private static string RemovePaymentButton(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;

            const string selector = "//*[@id = 'frm-invoice-submit-button']";
            var doc = new AG.HtmlDocument();
            doc.LoadHtml(html);
            var docNode = doc.DocumentNode;
            var sourceNode = docNode.SelectSingleNode(selector);
            if (sourceNode == null) return html;
            var txt = sourceNode.OuterHtml;
            html = html.Replace(txt, string.Empty);
            return html;
        }

        private static string StandarizeDescriptions(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            foreach (var kvp in descriptionReplacements)
            {
                if (html.Contains(kvp.Key))
                {
                    html = html.Replace(kvp.Key, kvp.Value);
                }
            }
            return html;
        }
    }
}