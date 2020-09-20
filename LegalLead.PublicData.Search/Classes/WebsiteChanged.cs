using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public class WebsiteChanged : WebsiteChangeEvent
    {
        public override string Name => CommonKeyIndexes.FutureKeyWord;

        protected TableLayoutPanel TableLayoutPanel1 => GetMain.tableLayoutPanel1;


        protected ComboBox CboCaseType => GetMain.cboCaseType;

        protected ComboBox CboWebsite => GetMain.cboWebsite;

        protected Label LabelCboCaseType => GetMain.labelCboCaseType;
        public override void Change()
        {
            const int FortyNine = 49;
            const int Zero = 0;
            const int Three = 3;
            const int Four = 4;
            const int Five = 5;
            base.Change();
            var source = (WebNavigationParameter)CboWebsite.SelectedItem;
            var customList = new List<int>
            {
                (int)SourceType.CollinCounty,
                (int)SourceType.TarrantCounty
            };

            var showList = new List<int> { Four, Five };
            CboCaseType.Visible = customList.Contains(source.Id);
            LabelCboCaseType.Text = source.Id == (int)SourceType.TarrantCounty 
                ? CommonKeyIndexes.CustomSearchLabel : CommonKeyIndexes.SearchTypeLabel;

            // get we do the custom labels here??
            MapLabels(TableLayoutPanel1.RowStyles);

            if (!customList.Contains(source.Id)) return;

            GetMain.tsStatusLabel.Text = new string(' ', 25);
            // custom combo mapping for case type
            var ccCaseName = CommonKeyIndexes.CollinCountyCaseType; // "collinCountyCaseType";
            var caseTypeName = source.Id == (int)SourceType.CollinCounty ?
                ccCaseName :
                CommonKeyIndexes.TarrantCountyCustomType; // "tarrantCountyCustomType";
            var caseTypes = CaseTypeSelectionDto.GetDto(caseTypeName);
            var dropDownOptions = caseTypes.DropDowns.First().Options;
            CboCaseType.DataSource = dropDownOptions;
            CboCaseType.DisplayMember = CommonKeyIndexes.NameProperCase; // "Name";
            CboCaseType.ValueMember = CommonKeyIndexes.IdProperCase;

            if (caseTypeName.Equals(ccCaseName, System.StringComparison.CurrentCultureIgnoreCase)) return;
            var selected = (Option)CboCaseType.SelectedItem;
            var expected = dropDownOptions.FirstOrDefault(o => o.IsDefault) ?? selected;
            if (selected.Id.Equals(expected.Id)) return;

            CboCaseType.SelectedItem = expected;

        }
    }
}
