using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search
{
    public partial class FsUserSettings : Form
    {
        public FsUserSettings()
        {
            InitializeComponent();
            txKeyValue.Enabled = false;
            btnSubmit.Enabled = false;
            _initalText = lbStatus.Text;
            _initalColor = lbStatus.ForeColor;
            isAdminAccount = IsAllCountyEnabled();
            var list = sourceData.FindAll(x =>
            {
                if (isAdminAccount) return isAdminAccount;
                return !x.IsSecured;
            });
            list.Sort((a, b) =>
            {
                var aa = a.Category.CompareTo(b.Category);
                if (aa != 0) return aa;
                return a.Name.CompareTo(b.Name);
            });
            _vwlist = list.JsonCast<List<UserSettingChangeViewModel>>();
            dataGridView1.DataSource = _vwlist;
            AppendSelectButton(dataGridView1);
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;
            btnSubmit.Click += BtnSubmit_Click;
            txKeyValue.TextChanged += TxKeyValue_TextChanged;
        }
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            var errorColor = Color.Red;
            lbStatus.Text = _initalText;
            lbStatus.ForeColor = _initalColor;
            var success = UserDataReader.Change(_model);
            if (!success)
            {
                lbStatus.ForeColor = errorColor;
                lbStatus.Text = "An error occurred processing change.";
                return;
            }
            var item = _vwlist.Find(x => x.Category == _model.Category &&
            x.Name == _model.Name);
            if (item != null) item.Value = _model.Value;

        }
        private void TxKeyValue_TextChanged(object sender, EventArgs e)
        {
            var actual = txKeyValue.Text;
            if (actual == null) return;
            _model.Value = actual;
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var isEnabled = false;

            try
            {
                _model.Category = string.Empty;
                _model.Name = string.Empty;
                _model.Value = string.Empty;
                txKeyValue.Enabled = isEnabled;
                if (e.RowIndex < 0) return;
                var senderGrid = (DataGridView)sender;
                if (senderGrid.Columns[e.ColumnIndex] is not DataGridViewButtonColumn) return;
                var item = _vwlist[e.RowIndex];
                if (item == null) return;

                _model.Category = item.Category;
                _model.Name = item.Name;
                _model.Value= item.Value;

                isEnabled = true;
                txKeyName.Text = _model.Name;
                txKeyValue.Text = _model.Value;
            }
            finally
            {
                btnSubmit.Enabled = isEnabled;
                txKeyValue.Enabled = isEnabled;
            }

        }

        private static void AppendSelectButton(DataGridView dataGridView1)
        {
            var columns = dataGridView1.Columns;
            if (columns.Count == 4) return;
            var append = new DataGridViewButtonColumn
            {
                HeaderText = "Select",
                Text = "Edit",
                ToolTipText = string.Empty,
                Width = 80
            };
            columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            for (var i = 0; i < columns.Count; i++) { 
                var col = columns[i];
                col.ReadOnly = true;
            }
            columns.Add(append);
            Debug.WriteLine("Grid view has : {0} columns", columns.Count);
        }

        private readonly string _initalText;
        private readonly Color _initalColor;
        private readonly bool isAdminAccount;
        private readonly UserSettingChangeViewModel _model = new();
        private readonly List<UserSettingChangeViewModel> _vwlist;
        private static bool IsAllCountyEnabled()
        {
            var webdetail = UserAccountReader.GetAccountPermissions();
            return webdetail.Equals("-1");
        }
        
        private static readonly ISessionPersistance UserAccountReader =
            SessionPersistenceContainer
            .GetContainer
            .GetInstance<ISessionPersistance>(ApiHelper.ApiMode);

        private static readonly SessionSettingPersistence UserDataReader =
            SessionPersistenceContainer
            .GetContainer
            .GetInstance<SessionSettingPersistence>();

        private static readonly List<UserSettingChangeModel> sourceData =
            UserDataReader.GetList<UserSettingChangeModel>();


    }
}
