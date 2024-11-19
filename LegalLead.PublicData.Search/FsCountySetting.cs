using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Extensions;
using System;
using System.Collections.Generic;
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
            list.Sort((a, b) => a.CountyName.CompareTo(b.CountyName));
            list.ForEach(itm =>
            {
                if (!vwlist.Exists(x => x.CountyName == itm.CountyName))
                {
                    vwlist.Add(new CountyDisplayItem
                    {
                        CountyName = itm.CountyName,
                        IsEnabled = itm.IsEnabled,
                        UserName = itm.UserName,
                        UserPassword = itm.UserPassword
                    });
                }
            });
            dataGridView1.DataSource = vwlist;

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
    }
}
