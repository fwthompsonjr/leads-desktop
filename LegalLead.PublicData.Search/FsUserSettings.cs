using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
                return a.Index.CompareTo(b.Index);
            });
            _vwlist = list.JsonCast<List<UserSettingChangeViewModel>>();
            dataGridView1.DataSource = _vwlist;
            AppendSelectButton(dataGridView1);
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;

            var keyvaluelist = new List<Control>()
            {
                txKeyValue,
                txKeyValue1,
                txKeyValue2,
                txKeyValue3
            };
            keyvaluelist.ForEach(x =>
            {
                x.TextChanged += TxKeyValue_TextChanged;
                x.DoubleClick += TxKeyValue_DoubleClick;
            });
            var controls = tableLayoutPanel1.Controls;
            foreach (var control in controls)
            {
                if (control is Button button)
                {
                    button.Click += BtnSubmit_Click;
                }
            }
            ToggleRow(DisplayMap.Keys.First());
        }
        private string DataFilterMode { get; set; } = string.Empty;
        private void ToggleRow(string displayMode)
        {
            DataFilterMode = displayMode;
            var list = new List<Control>();
            var rowStyles = tableLayoutPanel1.RowStyles;
            var controls = tableLayoutPanel1.Controls;
            var names = DisplayMap.Keys.ToList();
            foreach (Control control in controls)
            {
                if (control.Tag is string tagName
                    && names.Contains(tagName)) list.Add(control);
            }
            // hide any controls where name isnt displayName
            list.ForEach(ctl =>
            {
                var tagName = Convert.ToString(ctl.Tag) ?? string.Empty;
                var isVisible = displayMode.Equals(tagName);
                ctl.Visible = isVisible;
                if (!isVisible) ctl.Enabled = false;
            });
            foreach (var item in DisplayMap)
            {
                var rowHeight = item.Key.Equals(displayMode) ? 50 : 0;
                var style = rowStyles[item.Value];
                style.Height = rowHeight;
            }
        }

        private void ToggleTextBox(bool isEnabled)
        {
            var list = new List<Control>();
            var controls = tableLayoutPanel1.Controls;
            var names = DisplayMap.Keys.ToList();
            foreach (Control control in controls)
            {
                if (control.Tag is string tagName
                    && names.Contains(tagName)
                    && control is TextBox textBox) list.Add(textBox);
            }
            // hide any controls where name isnt displayName
            list.ForEach(ctl =>
            {
                if (ctl.Visible) ctl.Enabled = isEnabled;
            });
        }
        private void ToggleButton(bool isEnabled)
        {
            var list = new List<Control>();
            var controls = tableLayoutPanel1.Controls;
            var names = DisplayMap.Keys.ToList();
            foreach (Control control in controls)
            {
                if (control.Tag is string tagName
                    && names.Contains(tagName)
                    && control is Button bttn) list.Add(bttn);
            }
            // hide any controls where name isnt displayName
            list.ForEach(ctl =>
            {
                if (ctl.Visible) ctl.Enabled = isEnabled;
            });
        }

        private void SetKeyName(string text)
        {
            var list = new List<Control>()
            {
                txKeyName,
                txKeyName1,
                txKeyName2,
                txKeyName3
            };
            list.ForEach(ctl =>
            {
                ctl.Text = text;
            });
        }

        private void SetKeyValue(string text)
        {
            var list = new List<Control>()
            {
                txKeyValue,
                txKeyValue1,
                txKeyValue2,
                txKeyValue3
            };
            list.ForEach(ctl =>
            {
                ctl.Text = text;
            });
        }
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            var errorColor = Color.Red;
            lbStatus.Text = _initalText;
            lbStatus.ForeColor = _initalColor;
            if (string.IsNullOrEmpty(DataFilterMode))
            {

                lbStatus.ForeColor = errorColor;
                lbStatus.Text = "An error occurred processing change.";
                return;
            }
            var validator = ActionSettingContainer.GetContainer
                .GetInstance<ISettingChangeModel>(DataFilterMode);
            validator.Text = _model.Value;
            var collection = validator.Validate(out var isValid);
            if (!isValid)
            {
                var err = string.Join(Environment.NewLine, collection.Select(s => s.ErrorMessage));
                lbStatus.ForeColor = errorColor;
                lbStatus.Text = err;
                return;
            }
            var success = UserDataReader.Change(_model);
            if (!success)
            {
                lbStatus.ForeColor = errorColor;
                lbStatus.Text = "An error occurred processing change.";
                return;
            }
            var item = _vwlist.Find(x => x.Category == _model.Category &&
            x.Name == _model.Name);
            if (item == null) return;
            item.Value = _model.Value;
            dataGridView1.Refresh();
            lbStatus.Text = "Change completed successfully.";
        }
        private void TxKeyValue_TextChanged(object sender, EventArgs e)
        {
            if (sender is not TextBox tbox) return;
            var actual = tbox.Text;
            if (actual == null) return;
            _model.Value = actual;
        }

        private void TxKeyValue_DoubleClick(object sender, EventArgs e)
        {
            var values = new[] { "true", "false" };
            if (sender is not TextBox tbox) return;
            var actual = tbox.Text;
            if (actual == null) return;
            if (!values.Contains(actual)) return;
            var id = values.ToList().FindIndex(x => x.Equals(actual, StringComparison.OrdinalIgnoreCase));
            var toggle = id == 0 ? values[1] : values[0];
            tbox.Text = toggle;
            _model.Value = toggle;
        }
        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var isEnabled = false;

            try
            {
                _model.Category = string.Empty;
                _model.Name = string.Empty;
                _model.Value = string.Empty;
                ToggleTextBox(isEnabled);
                if (e.RowIndex < 0) return;
                var senderGrid = (DataGridView)sender;
                if (senderGrid.Columns[e.ColumnIndex] is not DataGridViewButtonColumn) return;
                var item = _vwlist[e.RowIndex];
                if (item == null) return;
                var src = sourceData.Find(x =>
                x.Category == item.Category &&
                x.Name == item.Name);
                var target = src?.DataType ?? DisplayMap.Keys.First();
                ToggleRow(target);

                _model.Category = item.Category;
                _model.Name = item.Name;
                _model.Value = item.Value;

                isEnabled = true;
                SetKeyName(_model.Name);
                SetKeyValue(_model.Value);
            }
            finally
            {
                ToggleTextBox(isEnabled);
                ToggleButton(isEnabled);
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
            for (var i = 0; i < columns.Count; i++)
            {
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

        private readonly List<UserSettingChangeModel> sourceData =
            UserDataReader.GetList<UserSettingChangeModel>();

        private static readonly Dictionary<string, int> DisplayMap
            = new()
            {
                { "Text", 2 },
                { "Bool", 3 },
                { "DateTime", 4 },
                { "Numeric", 5 },
            };

    }
}
