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

        /// <summary>
        /// Handles UI behavior on data grid row entered
        /// </summary>
        /// <param name="sender">the datagrid object</param>
        /// <param name="e"></param>
        private void DataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var rowCount = dataGridView1.RowCount;
            lbRecordCount.Text = GetRowCountLabel(e, rowCount);
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


    }
}