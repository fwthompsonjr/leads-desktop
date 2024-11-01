using LegalLead.PublicData.Search.Classes;
using System.Collections.Generic;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public class WebsiteChangeEvent : IWebsiteChangeEvent
    {
        public virtual string Name => @"Default";
        public FormMain GetMain { get; set; }
        public virtual void Change()
        {
            const int FortyNine = 49;
            const int Zero = 0;
            const int Three = 3;
            const int Five = 5;
            var boxindicies = new List<int> { DallasIndx, TravisIndx, BexarIndx, HidalgoIndx };
            var source = (WebNavigationParameter)GetMain.cboWebsite.SelectedItem;
            var cbo = GetMain.cboSearchType;
            var customBindingNeeded = boxindicies.Contains(source.Id);
            GetMain.ButtonDentonSetting.Visible = (
                source.Id == (int)SourceType.DentonCounty |
                source.Id == (int)SourceType.CollinCounty |
                source.Id == (int)SourceType.HarrisCriminal);
            GetMain.cboSearchType.Visible = source.Id == (int)SourceType.CollinCounty || customBindingNeeded;
            GetMain.cboCaseType.Visible = source.Id == (int)SourceType.CollinCounty;
            GetMain.cboCourts.Visible = source.Id == (int)SourceType.TarrantCounty;
            ToggleComboBoxBinding(cbo, customBindingNeeded, source.Id);
            var fives = new List<int>
            {
                (int)SourceType.CollinCounty,
                (int)SourceType.TarrantCounty
            };
            var styles = GetMain.tableLayoutPanel1.RowStyles;
            for (int i = Three; i <= Five; i++)
            {
                styles[i].SizeType = SizeType.Absolute;
                styles[i].Height = fives.Contains(source.Id) || i == 3 && customBindingNeeded
                    ? FortyNine : Zero;
                if (i == Five)
                {
                    styles[i].Height = fives.Contains(source.Id)
                        ? FortyNine :
                        Zero;
                }
            }
            GetMain.tsStatusLabel.Text = string.Empty;
            // when in Denton County write Settings
            if (source.Id == (int)SourceType.DentonCounty)
            {
                GetMain.ButtonDentonSetting.Text = CommonKeyIndexes.SettingsLabel;
                GetMain.SetDentonStatusLabelFromSetting();
            }
            else if (source.Id == (int)SourceType.HarrisCriminal)
            {
                GetMain.ButtonDentonSetting.Text = "Options";
            }
            else
            {
                GetMain.ButtonDentonSetting.Text = CommonKeyIndexes.PasswordLabel;
            }
            ApplyRowStyles(styles, source.Id);
        }

        private void ToggleComboBoxBinding(ComboBoxEx cbo, bool isCustomBindingNeeded, int countyId = 60)
        {
            var find = isCustomBindingNeeded ? "dallasCountyCaseOptions" : CommonKeyIndexes.CollinCountyCaseType;
            var caseTypes = CaseTypeSelectionDto.GetDto(find);
            var caseIndex = cbo.SelectedIndex;
            var customIndexes = new List<int> { TravisIndx, HidalgoIndx };
            // remove event handler
            cbo.SelectedIndexChanged -= GetMain.CboSearchType_SelectedIndexChanged;

            var dataSource = caseTypes.DropDowns.FindAll(x =>
            {
                if (!customIndexes.Contains(countyId)) return true;
                if (countyId == TravisIndx) return caseTypes.DropDowns.IndexOf(x) == 2;
                if (countyId == HidalgoIndx) return caseTypes.DropDowns.IndexOf(x) == 0;
                return true;
            });
            cbo.DataSource = dataSource;
            cbo.DisplayMember = CommonKeyIndexes.NameProperCase;
            cbo.ValueMember = CommonKeyIndexes.IdProperCase;
            cbo.SelectedIndex = caseIndex;

            // restore event handler
            cbo.SelectedIndexChanged += GetMain.CboSearchType_SelectedIndexChanged;
        }

        protected static void ApplyRowStyles(TableLayoutRowStyleCollection styles, int websiteId)
        {
            var styleProviders = RowStyleChangeProvider.RowChangeProviders;
            styleProviders.ForEach(p => p.ApplyStyles(styles, websiteId));
        }

        public void MapLabels(TableLayoutRowStyleCollection styles)
        {
            var source = (WebNavigationParameter)GetMain.cboWebsite.SelectedItem;
            var mapper = RowStyleChangeProvider.RowChangeProviders.Find(r => r.WebsiteIndex == source.Id);
            if (mapper == null) { return; }
            mapper.MapLabels(GetMain);
        }

        private const int DallasIndx = (int)SourceType.DallasCounty;
        private const int TravisIndx = (int)SourceType.TravisCounty;
        private const int BexarIndx = (int)SourceType.BexarCounty;
        private const int HidalgoIndx = (int)SourceType.HidalgoCounty;
    }
}
