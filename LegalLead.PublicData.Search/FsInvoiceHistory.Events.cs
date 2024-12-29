using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FsInvoiceHistory : Form
    {
        /// <summary>
        /// Handles Button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            SetDisplay(DisplayModes.Loading);
            var worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void BtnViewInvoice_Click(object sender, EventArgs e)
        {
            if (sender is not Button btn) return;

            lock (sync)
            {
                if (btn.Text.Equals(viewNormal))
                {
                    SetDisplay(DisplayModes.Normal);
                    btn.Text = viewInvoice;
                    return;
                }
                var html = WebContent;
                wbViewer.NavigateToString(html);
                SetDisplay(DisplayModes.Invoicing);
                wbViewer.NavigateToString(html);
                var selections = dataGridView1.SelectedCells.Cast<DataGridViewCell>().ToList();
                var rowIndex = selections.Min(x => x.RowIndex);
                var rows = dataGridView1.Rows.Cast<DataGridViewRow>().ToList();
                dataGridView1.FirstDisplayedScrollingRowIndex = rowIndex;
                rows.ForEach(rw =>
                {
                    var backColor = rw.Index == rowIndex ? System.Drawing.Color.Yellow : System.Drawing.Color.White;
                    rw.DefaultCellStyle.BackColor = backColor;
                });

                lbInvoiceName.Text = string.Empty;
                btnPayInvoice.Enabled = false;
                btnPayInvoice.Visible = true;
                btnPayInvoice.Tag = null;
                var statusReader = new InvoicePaymentStatusResponse(
                    dataGridView1,
                    dataGridView1.Rows[rowIndex]);

                lbInvoiceName.Text = statusReader.Text;
                btnPayInvoice.Enabled = statusReader.IsEnabled;
                if (statusReader.IsEnabled && !string.IsNullOrEmpty(statusReader.PaymentUrl))
                {
                    btnPayInvoice.Tag = statusReader.PaymentUrl;
                    if (statusReader.InvoiceItem is not null) AppendStatusCheck(statusReader.InvoiceItem);
                }
            }
        }

        private void BtnPayInvoice_Click(object sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not string navTo) return;
            if (!Uri.TryCreate(navTo, UriKind.Absolute, out var _)) return;
            wbViewer.CoreWebView2.Navigate(navTo);
        }

        /// <summary>
        /// Handles UI behavior on data grid row entered
        /// </summary>
        /// <param name="sender">the datagrid object</param>
        /// <param name="e"></param>
        private void DataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            InvoiceHtmlModel model = null;
            var indx = e.RowIndex;
            try
            {
                if (indx < 0) return;
                var rowCount = dataGridView1.RowCount;
                lbRecordCount.Text = GetRowCountLabel(e, rowCount);
                if (htmlData.Count == 0) return;
                if (dataGridView1.DataSource is not List<InvoiceHeaderViewModel> list) return;
                var selection = list[indx];
                model = htmlData.Find(x =>
                {
                    return x.Id == selection.Id;
                });
            }
            finally
            {
                if (model != null && indx >= 0)
                {
                    var row = dataGridView1.Rows[indx];
                    if (row.Tag == null) row.Tag = model;
                }
                SetInvoicingMode(model);
            }

        }

        /// <summary>
        /// Handles events after form is first displayed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FsInvoiceHistory_Shown(object sender, EventArgs e)
        {
            try
            {
                BtnSubmit_Click(null, null);
            }
            finally
            {
                var worker = new BackgroundWorker();
                worker.DoWork += RawData_DoWork;
                worker.RunWorkerCompleted += RawData_RunWorkerCompleted;
                worker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Menu item click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsOption_Click(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem itm) return;
            if (itm.Checked || itm.Text.Equals(none))
            {
                SelectFilterOption(none);
                dataGridView1.DataSource = _vwlist;
                dataGridView1.Refresh();
                SetDataCaption(all_counties, _vwlist);
                return;
            }
            var subset = _vwlist.FindAll(x => x.CountyName == itm.Text);
            dataGridView1.DataSource = subset;
            dataGridView1.Refresh();
            SelectFilterOption(itm.Text);
            SetDataCaption(itm.Text, subset);
            lbRecordCount.Text = GetRowCountLabel(1, subset.Count);
            SetDisplay(DisplayModes.Normal);
        }


        private void WbViewer_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess) return;
            WebContent = string.Empty;
            wbViewer.NavigateToString(WebContent);
        }


        private void WbViewer_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {

            if (wbViewer.Tag is not bool isVisible) { return; }
            wbViewer.Visible = isVisible;

        }

        private void WbViewer_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.NavigationKind != Microsoft.Web.WebView2.Core.CoreWebView2NavigationKind.NewDocument)
            {
                wbViewer.Tag = null;
                return;
            }
            if (e.Uri.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return;
            var isVisible = wbViewer.Visible;
            wbViewer.Tag = isVisible;
            wbViewer.Visible = false;
        }

        /// <summary>
        /// Toggles menu item state in response to item click event
        /// </summary>
        /// <param name="selection"></param>
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

        /// <summary>
        /// Sets the text of data status label
        /// </summary>
        /// <param name="countyName"></param>
        /// <param name="subset"></param>
        private void SetDataCaption(string countyName, List<InvoiceHeaderViewModel> subset)
        {
            var subsummary = new GridSummary
            {
                CountyName = countyName,
                RecordCount = subset.Count,
                SearchCount = subset.Sum(x => x.RecordCount),
            };
            var text = subsummary.GetCaptionText();
            tslbDataStatus.Text = text;
        }

        /// <summary>
        /// Method used to populate menu options with county names
        /// </summary>
        private void FormatCounties()
        {
            try
            {
                TryFormatCounties();
            }
            catch (Exception)
            {
                Invoke(() => TryFormatCounties());
            }
        }

        /// <summary>
        /// Method used to populate menu options with county names
        /// </summary>
        private void TryFormatCounties()
        {
            // add options to counties filter,
            // setting no filter as default
            var items = new List<string> { none };
            var collection = _vwlist.Select(x => x.CountyName).Distinct().ToList();
            collection.Sort();
            items.AddRange(collection);
            tssbFilterCounty.DropDownItems.Clear();
            items.ForEach(i =>
            {
                var tsOption = new ToolStripMenuItem(i)
                {
                    Checked = items.IndexOf(i) == 0
                };
                tsOption.Click += TsOption_Click;
                tssbFilterCounty.DropDownItems.Add(tsOption);
            });
        }

        private void SetInvoicingMode(InvoiceHtmlModel model)
        {
            try
            {
                TrySetInvoicingMode(model);
            }
            catch (Exception)
            {
                Invoke(() => TrySetInvoicingMode(model));
            }
        }

        private void TrySetInvoicingMode(InvoiceHtmlModel model)
        {
            var enabled = !string.IsNullOrWhiteSpace(model?.Html);
            var content = model?.Html ?? string.Empty;
            WebContent = content;
            if (!btnViewInvoice.Visible) btnViewInvoice.Visible = enabled;
            if (btnViewInvoice.Visible) btnPayInvoice.Visible = true;
            btnViewInvoice.Enabled = enabled;
            // get selected row idex
            if (dataGridView1.SelectedCells.Count <= 0) return;
            var cells = dataGridView1.SelectedCells.Cast<DataGridViewCell>().ToList();
            var indx = cells.Min(x => x.RowIndex);
            var statusReader = new InvoicePaymentStatusResponse(
                dataGridView1,
                dataGridView1.Rows[indx]);
            lbInvoiceName.Text = statusReader.Text;
        }

        /// <summary>
        /// Creates the row count indicator text based on grid row selection
        /// </summary>
        private static string GetRowCountLabel(DataGridViewCellEventArgs e, int rowCount)
        {
            var index = e.RowIndex + 1;
            return GetRowCountLabel(index, rowCount);
        }

        /// <summary>
        /// Creates the row count indicator text based on grid row selection
        /// </summary>
        private static string GetRowCountLabel(int index, int rowCount)
        {
            if (index == 0 || index > rowCount || rowCount == 0)
                return "Reading records";
            return $"Item {index} of {rowCount}";
        }

        private static readonly string blankHtmlContent = Properties.Resources.invoice_no_data;

        private sealed class InvoicePaymentStatusResponse
        {
            private readonly DataGridView grid;
            public InvoicePaymentStatusResponse(
                DataGridView source,
                DataGridViewRow sender)
            {
                grid = source;
                GetInvoiceTextFromIndex(sender.Index);
            }
            public string Text { get; private set; } = string.Empty;
            public string Status { get; private set; } = string.Empty;
            public string PaymentUrl { get; private set; } = string.Empty;
            public InvoiceHtmlModel InvoiceItem { get; private set; } = null;
            public bool IsEnabled
            {
                get
                {
                    if (string.IsNullOrEmpty(PaymentUrl)) return false;
                    if (string.IsNullOrEmpty(Status)) return false;
                    return Status.Equals("SENT");
                }
            }

            private void GetInvoiceTextFromIndex(int rowIndex)
            {
                var fallback = $"( getting details: {rowIndex} )";
                try
                {
                    if (grid.DataSource is not List<InvoiceHeaderViewModel> list) return;
                    if (htmlData.Count == 0) return;
                    if (rowIndex < 0 || rowIndex > list.Count - 1) return;
                    var current = list[rowIndex];
                    var target = htmlData.Find(x => x.Id == current.Id);
                    if (target == null || string.IsNullOrWhiteSpace(target.Caption)) return;
                    InvoiceItem = target;
                    fallback = target.Caption;
                    Status = target.Status;
                    PaymentUrl = target.PaymentUrl;
                }
                finally
                {
                    Text = fallback;
                }
            }
        }

    }
}