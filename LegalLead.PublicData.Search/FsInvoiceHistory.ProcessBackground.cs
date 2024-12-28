using LegalLead.PublicData.Search.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FsInvoiceHistory : Form
    {

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
            }
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
            list.ForEach(m =>
            {
                var invoiceData = m.Model.ToJsonString();
                var html = invoiceReader.PreviewInvoice(invoiceData);
                var item = new InvoiceHtmlModel
                {
                    CountyName = m.CountyName,
                    Description = m.Description,
                    RecordCount = m.RecordCount,
                    InvoiceDate = m.InvoiceDate,
                    Price = m.Price,
                    Html = html ?? string.Empty
                };
                target.Add(item);
            });
            e.Result = target;
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
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = GetHistory();
        }
        /// <summary>
        /// Fetch data from remote data source on store in IList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RawData_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = GetRawData();
        }

        /// <summary>
        /// Handles data-binding and user interface state after data completion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RawData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (sync)
            {
                if (e.Result is not List<GetUsageResponseContent> models) return;
                rawData.Clear();
                rawData.AddRange(models);
                AllowExcelRevision = true;
            }
        }

        /// <summary>
        /// Fetch data from remote data source on store in IList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Summary_DoWork(object sender, DoWorkEventArgs e)
        {
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
            }
        }
    }
}