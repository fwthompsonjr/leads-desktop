using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public class WebsiteChanged : WebsiteChangeEvent
    {
        public override string Name => @"Future";

        protected TableLayoutPanel TableLayoutPanel1 => GetMain.tableLayoutPanel1;


        protected ComboBox CboCaseType => GetMain.cboCaseType;

        protected ComboBox CboWebsite => GetMain.cboWebsite;

        protected Label LabelCboCaseType => GetMain.labelCboCaseType;
        public override void Change()
        {
            base.Change();
            var source = (WebNavigationParameter)CboWebsite.SelectedItem;
            var customList = new List<int>
            {
                (int)SourceType.CollinCounty,
                (int)SourceType.TarrantCounty
            };

            var showList = new List<int>
            {
                4,
                5
            };
            CboCaseType.Visible = customList.Contains(source.Id);
            LabelCboCaseType.Text = source.Id == (int)SourceType.TarrantCounty ? "Custom Search" : "Search Type";

            for (int i = 3; i <= 5; i++)
            {
                TableLayoutPanel1.RowStyles[i].SizeType = SizeType.Absolute;
                TableLayoutPanel1.RowStyles[i].Height = source.Id == (int)SourceType.CollinCounty ? 49 : 0;
                if (showList.Contains(i))
                {
                    TableLayoutPanel1.RowStyles[i].Height = source.Id == (int)SourceType.TarrantCounty ? 49 : 0;
                }
            }

            if (!customList.Contains(source.Id)) return;
            // custom combo mapping for case type
            var caseTypeName = source.Id == (int)SourceType.CollinCounty ?
                "collinCountyCaseType" :
                "tarrantCountyCustomType";
            var caseTypes = CaseTypeSelectionDto.GetDto(caseTypeName);
            CboCaseType.DataSource = caseTypes.DropDowns.First().Options;
            CboCaseType.DisplayMember = "Name";
            CboCaseType.ValueMember = "Id";
        }
    }
}
