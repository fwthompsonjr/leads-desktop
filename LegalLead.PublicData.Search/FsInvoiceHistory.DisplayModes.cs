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
                dataGridView1.Visible = uiEnabled;
                toolStrip1.Visible = uiEnabled;
                lbStatus.Text = text;
            }
        }


        private enum DisplayModes
        {
            None,
            Normal,
            Loading,
            Invoicing
        }
    }
}