using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Classes
{
    internal interface IRowStyleChanged
    {
        int WebsiteIndex { get; }

        void ApplyStyles(TableLayoutRowStyleCollection styles, int websiteId);
        void MapLabels(FormMain main);
    }

    internal abstract class RowStyleChangeBase : IRowStyleChanged
    {

        protected class LabelSetting
        {
            public string OldText { get; set; }
            public string ChangedText { get; set; }

            public Label Target { get; set; }
        }

        public abstract int WebsiteIndex { get; }
        public abstract List<int> HiddenRows { get; }


        public void ApplyStyles(TableLayoutRowStyleCollection styles, int websiteId)
        {
            if (websiteId != WebsiteIndex) return;
            if (styles == null) return;
            if (HiddenRows == null) return;
            if (!HiddenRows.Any()) return;
            HiddenRows.ForEach(r => styles[r].Height = 0);
        }

        public virtual void MapLabels(FormMain main)
        {

            var changes = new List<LabelSetting>() {
                new LabelSetting { Target = main.labelCboCaseType },
                new LabelSetting { Target = main.label4 }
                };
            changes.ForEach(c =>
            {
                if (c.Target.Tag != null)
                {
                    c.Target.Text = Convert.ToString(c.Target.Tag);
                }
            });
            // reset that combo box here??

            var caseTypes = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.CollinCountyCaseType);
            main.cboSearchType.DataSource = caseTypes.DropDowns;
            main.cboSearchType.DisplayMember = CommonKeyIndexes.NameProperCase;
            main.cboSearchType.ValueMember = CommonKeyIndexes.IdProperCase;

            main.cboCaseType.DataSource = caseTypes.DropDowns.First().Options;
            main.cboCaseType.DisplayMember = CommonKeyIndexes.NameProperCase;
            main.cboCaseType.ValueMember = CommonKeyIndexes.IdProperCase;

        }
    }

    internal class HarrisCivilRowStyleChange : RowStyleChangeBase
    {
        public override int WebsiteIndex => (int)SourceType.HarrisCivil;

        public override List<int> HiddenRows => new List<int> { 3, 4 };

        public override void MapLabels(FormMain main
            // TableLayoutRowStyleCollection styles
            )
        {
            // labelCboCaseType
            var styles = main.tableLayoutPanel1.RowStyles;
            styles[3].Height = 50;
            styles[4].Height = 50;
            var changes = new List<LabelSetting>() {
                new LabelSetting { OldText = "Case Type", ChangedText = "Courts", Target = main.labelCboCaseType },
                new LabelSetting { OldText = "Search Type", ChangedText = "Status", Target = main.label4 }
                };
            changes.ForEach(c =>
            {
                c.Target.Tag = c.OldText;
                c.Target.Text = c.ChangedText;
            });
            main.labelCboCaseType.Text = "Courts";
            main.label4.Text = "Status";
            // cboSearchType

            var ccCaseName = CommonKeyIndexes.HarrisCivilCaseType;
            var caseTypes = CaseTypeSelectionDto.GetDto(ccCaseName);
            var cbxCase = main.cboCaseType;
            var selections = caseTypes.DropDowns[2];

            cbxCase.DataSource = caseTypes.DropDowns.First().Options;
            cbxCase.DisplayMember = CommonKeyIndexes.NameProperCase;
            cbxCase.ValueMember = CommonKeyIndexes.IdProperCase;
            cbxCase.SelectedIndex = Convert.ToInt32(
                selections.Options[0].Query,
                CultureInfo.CurrentCulture);
            cbxCase.Visible = true;

            cbxCase = main.cboSearchType;
            cbxCase.DataSource = caseTypes.DropDowns[1].Options.ToDropDown();
            cbxCase.DisplayMember = CommonKeyIndexes.NameProperCase;
            cbxCase.ValueMember = CommonKeyIndexes.IdProperCase;
            cbxCase.SelectedIndex = Convert.ToInt32(
                selections.Options[1].Query,
                CultureInfo.CurrentCulture);
            cbxCase.Visible = true;

        }
    }

    internal class TarrantRowStyleChange : RowStyleChangeBase
    {
        public override int WebsiteIndex => (int)SourceType.TarrantCounty;

        public override List<int> HiddenRows => new List<int> { 3, 4 };

    }

    internal class CollinRowStyleChange : RowStyleChangeBase
    {
        public override int WebsiteIndex => (int)SourceType.CollinCounty;

        public override List<int> HiddenRows => new List<int> { 4 };

    }

    internal class DentonRowStyleChange : RowStyleChangeBase
    {
        public override int WebsiteIndex => (int)SourceType.DentonCounty;

        public override List<int> HiddenRows => new List<int> { 3, 4 };

    }

    internal static class RowStyleChangeProvider
    {
        private static List<IRowStyleChanged> _providers;
        internal static List<IRowStyleChanged> RowChangeProviders
        {
            get { return _providers ?? (_providers = GetProviders()); }
        }

        private static List<IRowStyleChanged> GetProviders()
        {
            return new List<IRowStyleChanged>
            {
                new DentonRowStyleChange(),
                new CollinRowStyleChange(),
                new TarrantRowStyleChange(),
                new HarrisCivilRowStyleChange()
            };
        }
    }
}
