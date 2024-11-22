using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FsCountySetting : Form
    {
        public FsCountySetting()
        {
            InitializeComponent();
            sourceTypes.ForEach(type =>
            {
                var name = type.GetCountyName();
                var id = (int)type;
                list.Add(new() { CountyId = id, CountyName = name });
            });
            var allowAllCounties = IsAllCountyEnabled();
            list.Sort((a, b) => a.CountyName.CompareTo(b.CountyName));
            list.ForEach(itm =>
            {
                var setting = GetCountyCredential(itm.CountyName);
                itm.UserName = setting[0];
                itm.UserPassword = setting[1];
                if (!vwlist.Exists(x => x.CountyName == itm.CountyName))
                {
                    vwlist.Add(new CountyDisplayItem
                    {
                        CountyName = itm.CountyName,
                        IsEnabled = IsCountyEnabled(allowAllCounties, itm.CountyId),
                        UserName = itm.UserName,
                        UserPassword = GetMasked(itm.UserPassword)
                    });
                }
            });
            if (!allowAllCounties) vwlist.RemoveAll(x => !x.IsEnabled);
            dataGridView1.DataSource = vwlist;
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;

            btnSubmit.Enabled = false;
            txUserName.Enabled = false;
            txPassword.Enabled = false;

            _initalText = lbStatus.Text;
            _initalColor = lbStatus.ForeColor;
        }
        private readonly string _initalText;
        private readonly Color _initalColor;
        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var isEnabled = false;

            try
            {
                _model.UserName = string.Empty;
                _model.Password = string.Empty;
                _model.CountyName = string.Empty;

                if (e.RowIndex < 0) return;
                var senderGrid = (DataGridView)sender;
                if (senderGrid.Columns[e.ColumnIndex] is not DataGridViewButtonColumn) return;
                var item = vwlist[e.RowIndex];
                if (item == null || !item.IsEnabled) return;
                var refitem = list.Find(x => x.CountyName.Equals(item.CountyName));
                if (refitem == null) return;

                isEnabled = true;
                txUserName.Text = refitem.UserName;
                txPassword.Text = refitem.UserPassword;

                _model.UserName = refitem.UserName;
                _model.Password = refitem.UserPassword;
                _model.CountyName = refitem.CountyName;
            }
            finally
            {
                btnSubmit.Enabled = isEnabled;
                txUserName.Enabled = isEnabled;
                txPassword.Enabled = isEnabled;
            }

        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            var errorColor = Color.Red;
            lbStatus.Text = _initalText;
            lbStatus.ForeColor = _initalColor;
            _model.UserName = txUserName.Text;
            _model.Password = txPassword.Text;
            var errors = _model.Validate(out var isPassed);
            if (!isPassed)
            {
                lbStatus.ForeColor = errorColor;
                lbStatus.Text = string.Join(Environment.NewLine, errors.Select(s => s.ErrorMessage));
                return;
            }
            // make api call
            var success = CountyService.ChangePassword(_model);
            if (success)
            {
                // update list objects
                var bounditem = vwlist.Find(x => x.CountyName == _model.CountyName);
                if (bounditem != null)
                {
                    bounditem.UserName = txUserName.Text;
                    bounditem.UserPassword = GetMasked(txPassword.Text);
                }
                var subset = list.FindAll(x => x.CountyName == _model.CountyName);
                subset.ForEach(s =>
                {
                    s.UserName = txUserName.Text;
                    s.UserPassword = txPassword.Text;
                });
            }
            else
            {
                lbStatus.ForeColor = errorColor;
            }
            var message = success ? "Password change successfully" : "An error occurred processing request";
            lbStatus.Text = message;
        }
        private static bool IsAllCountyEnabled()
        {
            var webdetail = UserAccountReader.GetAccountPermissions();
            return webdetail.Equals("-1");
        }

        private static bool IsCountyEnabled(bool isAllCounty, int countyId)
        {
            if (isAllCounty) return true;
            var webdetail = UserAccountReader.GetAccountPermissions();

            var webid = webdetail.Split(',')
                .Where(w => { return int.TryParse(w, out var _); })
                .Select(s => int.Parse(s, CultureInfo.CurrentCulture))
                .ToList();
            return webid.Contains(countyId);
        }
        private static string[] GetCountyCredential(string county)
        {
            const char pipe = '|';
            var model = UserAccountReader.GetAccountCredential(county);
            if (string.IsNullOrEmpty(model) || !model.Contains(pipe)) return new[] { "", "" };
            return model.Split(pipe);
        }
        private static string GetMasked(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            var len = str.Length;
            return string.Concat(Enumerable.Repeat("*", len));
        }
        private sealed class CountyDisplayItem
        {

            public string CountyName { get; set; }
            public bool IsEnabled { get; set; }
            public string UserName { get; set; }
            public string UserPassword { get; set; }
            public string ButtonText { get; } = "Change";
        }

        private readonly UserCountyPasswordModel _model = new();
        private readonly List<CountyDisplayItem> vwlist = new();
        private readonly List<CountyPermissionViewModel> list = new();
        private static readonly List<SourceType> sourceTypes = Enum.GetValues<SourceType>().ToList();
        private static readonly ISessionPersistance UserAccountReader =
            SessionPersistenceContainer
            .GetContainer
            .GetInstance<ISessionPersistance>(ApiHelper.ApiMode);
        private static readonly UserCountyPasswordService CountyService =
            SessionPersistenceContainer
            .GetContainer
            .GetInstance<UserCountyPasswordService>();
    }
}
