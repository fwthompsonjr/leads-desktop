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
            if (websiteId != WebsiteIndex)
            {
                return;
            }

            if (styles == null)
            {
                return;
            }

            if (HiddenRows == null)
            {
                return;
            }

            if (!HiddenRows.Any())
            {
                return;
            }

            HiddenRows.ForEach(r => styles[r].Height = 0);
        }

        public virtual void MapLabels(FormMain main)
        {

            var changes = new List<LabelSetting>() {
                new() { Target = main.labelCboCaseType },
                new() { Target = main.label4 }
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

        public override List<int> HiddenRows => new() {
            RowsIndexes.SearchTypeId, // 3, 
            RowsIndexes.CaseTypeId, // 4 
        };

        public override void MapLabels(FormMain main
            // TableLayoutRowStyleCollection styles
            )
        {
            // labelCboCaseType
            var styles = main.tableLayoutPanel1.RowStyles;
            styles[RowsIndexes.SearchTypeId].Height = 50;
            styles[RowsIndexes.CaseTypeId].Height = 50;
            var changes = new List<LabelSetting>() {
                new() { OldText = "Case Type", ChangedText = "Courts", Target = main.labelCboCaseType },
                new() { OldText = "Search Type", ChangedText = "Status", Target = main.label4 }
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

            cbxCase.DataSource = caseTypes.DropDowns[0].Options;
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

        public override List<int> HiddenRows => new() {
            RowsIndexes.SearchTypeId, // 3, 
            RowsIndexes.CaseTypeId, // 4
        };

    }

    internal class CollinRowStyleChange : RowStyleChangeBase
    {
        public override int WebsiteIndex => (int)SourceType.CollinCounty;

        public override List<int> HiddenRows => new() { RowsIndexes.CaseTypeId };

    }

    internal class DentonRowStyleChange : RowStyleChangeBase
    {
        public override int WebsiteIndex => (int)SourceType.DentonCounty;

        public override List<int> HiddenRows => new() {
            RowsIndexes.SearchTypeId, // 3, 
            RowsIndexes.CaseTypeId, // 4
        };

    }

    internal static class RowStyleChangeProvider
    {
        private static List<IRowStyleChanged> _providers;
        internal static List<IRowStyleChanged> RowChangeProviders
        {
            get { return _providers ??= GetProviders(); }
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


    internal class RowStyleDefinition
    {
        public virtual SizeType Size { get; set; } = SizeType.Absolute;
        public virtual float Height { get; set; } = 46f;
        public void ApplyStyle(TableLayoutRowStyleCollection styles, int index) 
        { 
            var count = styles.Count - 1;
            if (index < 0 || index > count) { return; }
            var style = styles[index];
            style.SizeType = Size;
            style.Height = Height;
        }
    }

    internal class MessageRowStyleDefinition : RowStyleDefinition
    {
        public override SizeType Size { get; set; } = SizeType.Percent;
        public override float Height { get; set; } = 100f;
    }
    internal class SpacerRowStyleDefinition : RowStyleDefinition
    {
        public override SizeType Size { get; set; } = SizeType.Absolute;
        public override float Height { get; set; } = 10f;
    }
    internal class MenuRowStyleDefinition : RowStyleDefinition
    {
        public override SizeType Size { get; set; } = SizeType.Absolute;
        public override float Height { get; set; } = 40f;
    }
    internal class HiddenRowStyleDefinition : RowStyleDefinition
    {
        public override float Height { get; set; } = 40f;
    }
    internal class LogStatusRowStyleDefinition : RowStyleDefinition
    {
        public override SizeType Size { get; set; } = SizeType.Percent;
        public override float Height { get; set; } = 100f;
    }
    internal static class RowsIndexes
    {
        public const int TopMenuId = 0;
        public const int WebsiteRowId = 1;
        public const int StartDateId = 2;
        public const int EndDateId = 3;
        public const int SearchTypeId = 4;
        public const int CaseTypeId = 5;
        public const int CaseTypeAdditionaId = 6;
        public const int ButtonRowId = 7;
        public const int MessageRowId = 8;
        public const int ProgressRowId = 9;
        public const int NotesRowId = 10;
        public const int SpacerRowId = 11;
        public const int ButtomMenuId = 12;
    }
    internal class DefaultStyleCollection
    {
        private readonly FormMain Context;
        public DefaultStyleCollection(FormMain main)
        {
            Context = main;
        }
        public void Apply()
        {
            var styles = Context.tableLayoutPanel1.RowStyles;
            for (int i = 0; i < styles.Count; i++) {
                defaultStyle[i].ApplyStyle(styles, i);
            }
        }
        static readonly Dictionary<int, RowStyleDefinition> defaultStyle = new()
        {
            {RowsIndexes.TopMenuId, new MenuRowStyleDefinition() },
            {RowsIndexes.WebsiteRowId, new RowStyleDefinition() },
            {RowsIndexes.StartDateId, new RowStyleDefinition() },
            {RowsIndexes.EndDateId, new RowStyleDefinition() },
            {RowsIndexes.SearchTypeId, new HiddenRowStyleDefinition() },
            {RowsIndexes.CaseTypeId, new HiddenRowStyleDefinition() },
            {RowsIndexes.CaseTypeAdditionaId, new HiddenRowStyleDefinition() },
            {RowsIndexes.ButtonRowId, new MenuRowStyleDefinition() },
            {RowsIndexes.MessageRowId, new MessageRowStyleDefinition() },
            {RowsIndexes.ProgressRowId, new HiddenRowStyleDefinition() },
            {RowsIndexes.NotesRowId, new HiddenRowStyleDefinition() },
            {RowsIndexes.SpacerRowId, new SpacerRowStyleDefinition() },
            {RowsIndexes.ButtomMenuId, new MenuRowStyleDefinition() },
        };
    }
}
