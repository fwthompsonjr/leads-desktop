using System;
using System.Globalization;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FsOfflineHistory
    {
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (sender is not System.Windows.Forms.Button btn) return;
            if (!btn.Enabled) return;
            try
            {
                btn.Enabled = false;
                BindRecords();
            }
            finally
            {
                btn.Enabled = true;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (Program.mainForm == null) return;
            Program.mainForm.menuOffline.PerformClick();
        }

        private void FsOfflineHistory_Shown(object sender, EventArgs e)
        {
            grid.Visible = false;
            BindRecords();
            grid.Visible = true;
        }

        private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!grid.Enabled) return;
            try
            {
                grid.Enabled = false;
                if (e.RowIndex < 0) return;
                if (grid.Columns[e.ColumnIndex].Name != downLoad) return;
                if (CanOpenFile(e.RowIndex, e.ColumnIndex))
                {
                    OpenFile(e.RowIndex);
                    return;
                }
                GenerateContent(e.RowIndex);
            }
            finally
            {
                grid.Enabled = true;
            }
        }


        private void Grid_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!grid.Enabled) return;
            try
            {
                if (e.RowIndex < 0) return;
                var rowId = e.RowIndex;
                var rc = grid.Rows.Count;
                // Record x of y : Dallas Justice
                lbStatus.Text = $"Record: {rowId + 1} of {rc}";
                if (grid.Rows[rowId].DataBoundItem is not GridHistoryView itm) return;
                lbStatus.Text = $"Record: {rowId + 1} of {rc}, {itm.CountyName} {textConverter.ToTitleCase(itm.CourtType.ToLower())} {itm.DatesSearched}";
            }
            finally
            {
                grid.Enabled = true;
            }
        }
        private static readonly TextInfo textConverter = new CultureInfo("en-US", false).TextInfo;
    }
}
