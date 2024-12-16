using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FsSearchHistory : Form
    {
        public FsSearchHistory()
        {
            InitializeComponent();
            _vwlist = new();
            FormatGrid(dataGridView1);
            btnSubmit.Click += BtnSubmit_Click;
            Shown += FsSearchHistory_Shown;
        }

        private void FsSearchHistory_Shown(object sender, EventArgs e)
        {
            BtnSubmit_Click(null, null);
        }
        /// <summary>
        /// Handles Button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            btnSubmit.Enabled = false;
            dataGridView1.Visible = false;
            lbStatus.Text = "Getting Search History ...";
            var worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }
        /// <summary>
        /// Handles data-binding and user interface state after data completion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is List<UsageHistoryViewModel> users)
            {
                _vwlist.Clear();
                _vwlist.AddRange(users);
            }
            dataGridView1.DataSource = _vwlist;
            dataGridView1.Refresh();
            FormatGrid(dataGridView1);
            btnSubmit.Enabled = true;
            dataGridView1.Visible = true;
            lbStatus.Text = "Ready";
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

        private static void FormatGrid(DataGridView dataGrid)
        {
            Padding padding = Padding.Empty;
            padding.Left = 3;
            padding.Right = 3;
            var columns = dataGrid.Columns;
            if (columns.Count == 0) return;
            columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            columns[0].Resizable = DataGridViewTriState.False;
            columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            for (var i = 0; i < columns.Count; i++)
            {
                var col = columns[i];
                col.ReadOnly = true;
                col.DefaultCellStyle.Padding = padding;
            }
        }
        private readonly List<UsageHistoryViewModel> _vwlist;
        private static List<UsageHistoryViewModel> GetHistory()
        {
            var data = usageReader.GetUsage();
            var list = new List<UsageHistoryModel>();
            if (data != null && data.Count > 0) { list.AddRange(data); }
            if (list.Count == 0) return new List<UsageHistoryViewModel>();
            list.Sort((a, b) =>
            {
                var aa = a.CountyName.CompareTo(b.CountyName);
                if (aa != 0) return aa;
                return b.CreateDate.CompareTo(a.CreateDate);
            });
            var models = list.Select(s => UsageHistoryViewModel.ConvertFrom(s));
            return models.ToList();
        }

        private static readonly SessionUsageReader usageReader = SessionPersistenceContainer
        .GetContainer
        .GetInstance<SessionUsageReader>();
    }
}
