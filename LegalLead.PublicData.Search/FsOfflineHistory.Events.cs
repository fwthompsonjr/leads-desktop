using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;

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

    }
}
