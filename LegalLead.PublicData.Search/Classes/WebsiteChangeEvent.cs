using LegalLead.PublicData.Search.Classes;
using System.Collections.Generic;
using System.Linq;
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
            var source = (WebNavigationParameter)GetMain.cboWebsite.SelectedItem;
            var cbo = GetMain.cboSearchType;
            var customBindingNeeded = boxindicies.Contains(source.Id);
            var defaultDef = new DefaultStyleCollection(GetMain);
            GetMain.ButtonDentonSetting.Visible = (
                source.Id == (int)SourceType.DentonCounty |
                source.Id == (int)SourceType.CollinCounty |
                source.Id == (int)SourceType.HarrisCriminal);
            GetMain.cboSearchType.Visible = source.Id == (int)SourceType.CollinCounty || customBindingNeeded;
            GetMain.cboCaseType.Visible = source.Id == (int)SourceType.CollinCounty;
            GetMain.cboCourts.Visible = source.Id == (int)SourceType.TarrantCounty;
            ToggleComboBoxBinding(cbo, customBindingNeeded, source.Id);
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
            defaultDef.Apply();
            HideProgress();
        }

        protected void HideProgress()
        {
            const string progressBar = "progressBar1";
            var controls = GetMain.Controls.Find(progressBar, true).ToList();
            controls.ForEach(control => { control.Visible = false; });
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
            try
            {
                cbo.SelectedIndex = caseIndex;
            }
            catch
            {
                cbo.SelectedIndex = 0;
            }


            // restore event handler
            cbo.SelectedIndexChanged += GetMain.CboSearchType_SelectedIndexChanged;
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
        private const int ElPasoIndx = (int)SourceType.ElPasoCounty;
        private const int FortBendIndx = (int)SourceType.FortBendCounty;
        private const int WilliamsonIndx = (int)SourceType.WilliamsonCounty;
        private const int GraysonIndx = (int)SourceType.GraysonCounty;
        private static readonly List<int> boxindicies = new() {
            DallasIndx,
            TravisIndx,
            BexarIndx,
            HidalgoIndx,
            ElPasoIndx,
            FortBendIndx,
            WilliamsonIndx,
            GraysonIndx
        };
    }
}
