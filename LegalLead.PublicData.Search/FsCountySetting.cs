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
                list.Sort((a, b) => a.CountyName.CompareTo(b.CountyName));
                dataGridView1.DataSource = list;
            });

        }


        private readonly List<CountyPermissionViewModel> list = new();
        private static readonly List<SourceType> sourceTypes = Enum.GetValues<SourceType>().ToList();
    }
}
