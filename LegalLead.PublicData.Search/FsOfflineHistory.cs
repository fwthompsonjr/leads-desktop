using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search
{
    public partial class FsOfflineHistory : Form
    {
        private readonly string leadUserId;

        public FsOfflineHistory(string leadId = "")
        {
            InitializeComponent();
            if (!Guid.TryParse(leadId, out var _))
            {
                leadId = UserAccountReader.GetAccountId();
            }
            leadUserId = leadId;
            BindRecords();
            grid.CellContentClick += Grid_CellContentClick;
        }

        private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!grid.Enabled) return;
            try
            {   
                grid.Enabled = false;
                if (e.RowIndex < 0) return;
                if (grid.Columns[e.ColumnIndex].Name != "Download") return;
                if (grid.DataSource is not List<GridHistoryView> accounts) return;
                var item = accounts[e.ColumnIndex];
                if (!item.IsComplete) return;
            }
            finally
            {
                grid.Enabled = true;
            }
        }

        private void BindRecords()
        {
            var data = ProcessOfflineHelper.GetRequests(leadUserId);
            var view = new List<GridHistoryView>();
            data.ForEach(d => view.Add(new(d)));
            grid.DataSource = null;
            grid.Tag = null;
            grid.Tag = data.ToJsonString();
            grid.DataSource = view;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ReadOnly = true;
            DataGridViewButtonColumn buttonColumn = new()
            {
                HeaderText = "Download",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(buttonColumn);
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.Refresh();
        }

        private class GridHistoryView(OfflineStatusResponse source)
        {
            public DateTime? StartDate { get; set; } = source.CreateDate;
            public string CountyName { get; set; } = source.CountyName;
            public string CourtType { get; set; } = source.CourtType ?? "-";
            public string DatesSearched { get; set; } = GetDateRange(source);
            public bool IsComplete { get; set; } = source.IsCompleted;
            public decimal PercentComplete { get; set; } = source.PercentComplete.GetValueOrDefault();
            public int RecordCount { get; set; } = source.RecordCount;
            public DateTime? LastUpdate { get; set; } = source.LastUpdate;
        }

        private static string GetDateRange(OfflineStatusResponse source)
        {
            if (!source.SearchStartDate.HasValue) return "-";
            var response = $"{source.SearchStartDate.Value:d}";
            if (source.SearchEndDate.HasValue)
            {
                response = $"{response} to {source.SearchEndDate:d}";
            }
            return response;
        }

        private static readonly ISessionPersistance UserAccountReader =
            SessionPersistenceContainer
            .GetContainer
            .GetInstance<ISessionPersistance>(ApiHelper.ApiMode);

        private void btnSubmit_Click(object sender, EventArgs e)
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
    }
}
