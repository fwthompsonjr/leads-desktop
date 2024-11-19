using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;
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
            dataGridView1.DataSource = vwlist;

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
            if (string.IsNullOrEmpty(model) || !model.Contains(pipe)) return new[] {"", ""};
            return model.Split(pipe);
        }
        private static string GetMasked(string str)
        {
            if (string.IsNullOrEmpty (str)) return "";
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

        private readonly List<CountyDisplayItem> vwlist = new();
        private readonly List<CountyPermissionViewModel> list = new();
        private static readonly List<SourceType> sourceTypes = Enum.GetValues<SourceType>().ToList(); 
        private static ISessionPersistance UserAccountReader =
            SessionPersistenceContainer
            .GetContainer
            .GetInstance<ISessionPersistance>(ApiHelper.ApiMode);
    }
}
