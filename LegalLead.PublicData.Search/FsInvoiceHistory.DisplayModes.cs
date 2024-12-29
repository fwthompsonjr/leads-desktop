using System.Collections.Generic;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FsInvoiceHistory : Form
    {
        /// <summary>
        /// Format data grid view column resettings
        /// </summary>
        /// <param name="dataGrid"></param>
        private static void FormatGrid(DataGridView dataGrid)
        {
            Padding padding = Padding.Empty;
            padding.Left = 3;
            padding.Right = 3;
            var columns = dataGrid.Columns;
            if (columns.Count == 0) return;
            dataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            const int columnUuid = 0;
            const int columnCountyName = 1;
            const int columnTitle = 2;
            const int columnRecordCount = 3;
            const int columnInvoiceDate = 4;
            const int columnInvoicePrice = 5;

            columns[columnUuid].Visible = false;

            columns[columnCountyName].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            columns[columnCountyName].Resizable = DataGridViewTriState.False;

            columns[columnTitle].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            columns[columnTitle].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            columns[columnRecordCount].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            columns[columnInvoiceDate].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            
            columns[columnInvoicePrice].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            columns[columnInvoicePrice].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            
            for (var i = 1; i < columns.Count; i++)
            {
                var col = columns[i];
                col.ReadOnly = true;
                col.DefaultCellStyle.Padding = padding;
            }
        }

        /// <summary>
        /// Method to toggle visibility of ui elements
        /// </summary>
        /// <param name="displayMode"></param>
        private void SetDisplay(DisplayModes displayMode)
        {
            try
            {
                TrySetDisplay(displayMode);
            }
            catch
            {
                Invoke(() =>
                {
                    TrySetDisplay(displayMode);
                });
            }
        }

        /// <summary>
        /// Method to toggle visibility of ui elements
        /// </summary>
        /// <param name="mode"></param>
        private void TrySetDisplay(DisplayModes mode)
        {
            lock (sync)
            {
                CurrentDisplay = mode;
                var uiEnabled = mode != DisplayModes.Loading;
                var text = (mode) switch
                {
                    DisplayModes.Loading => messageLoading,
                    _ => messageNormal
                };

                btnSubmit.Enabled = uiEnabled && AllowDataRefresh;
                btnViewInvoice.Visible = uiEnabled && AllowPreviewInvoice;
                dataGridView1.Visible = uiEnabled;
                toolStrip1.Visible = uiEnabled;
                lbStatus.Text = text;
                lbInvoiceName.Text = string.Empty;
                ToggleTableRows(mode == DisplayModes.Invoicing);
            }
        }

        private void ToggleTableRows(bool isInvoicing)
        {
            List<Control> controls = new()
            {
                dataGridView1,
                wbViewer
            };
            controls.ForEach(c => { 
                var currentId = controls.IndexOf(c);
                var rowIndex = tableLayoutPanel1.GetRow(c);
                var style = tableLayoutPanel1.RowStyles[rowIndex];
                var isVisible = 
                    (currentId == RowDataId && !isInvoicing) ||
                    (currentId == RowViewerId && isInvoicing);
                if (currentId == RowViewerId) { c.Visible = isVisible; }
                SetRowStyle(isVisible, currentId, style);
            });
        }

        private static void SetRowStyle(bool isVisible, int targetIndex, RowStyle style)
        {
            if (isVisible) { 
                style.SizeType = SizeType.Percent;
                style.Height = 100;
                return;
            }
            style.SizeType = SizeType.Absolute;
            style.Height = targetIndex == RowDataId ? 160 : 0;
        }

        private enum DisplayModes
        {
            None,
            Normal,
            Loading,
            Invoicing
        }
        private const int RowDataId = 0;
        private const int RowViewerId = 1;
    }
}