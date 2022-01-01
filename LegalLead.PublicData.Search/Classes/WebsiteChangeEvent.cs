using LegalLead.PublicData.Search.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
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
            var source = (WebNavigationParameter)GetMain.cboWebsite.SelectedItem;
            GetMain.ButtonDentonSetting.Visible = (
                source.Id == (int)SourceType.DentonCounty |
                source.Id == (int)SourceType.CollinCounty |
                source.Id == (int)SourceType.HarrisCriminal);
            GetMain.cboSearchType.Visible = source.Id == (int)SourceType.CollinCounty;
            GetMain.cboCaseType.Visible = source.Id == (int)SourceType.CollinCounty;
            GetMain.cboCourts.Visible = source.Id == (int)SourceType.TarrantCounty;
            var fives = new List<int>
            {
                (int)SourceType.CollinCounty,
                (int)SourceType.TarrantCounty
            };
            var styles = GetMain.tableLayoutPanel1.RowStyles;
            for (int i = Three; i <= Five; i++)
            {
                styles[i].SizeType = SizeType.Absolute;
                styles[i].Height = fives.Contains(source.Id)
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
                GetMain.ButtonDentonSetting.Text = CommonKeyIndexes.SettingsLabel; // "Settings";
                GetMain.SetDentonStatusLabelFromSetting();
            }
            else if(source.Id == (int)SourceType.HarrisCriminal)
            {
                GetMain.ButtonDentonSetting.Text = "Options";
            }
            else
            {
                GetMain.ButtonDentonSetting.Text = CommonKeyIndexes.PasswordLabel; // "Password";
            }
            ApplyRowStyles(styles, source.Id);
        }

        protected static void ApplyRowStyles(TableLayoutRowStyleCollection styles, int websiteId)
        {
            var styleProviders = RowStyleChangeProvider.RowChangeProviders;
            styleProviders.ForEach(p => p.ApplyStyles(styles, websiteId));
        }

        public void MapLabels(TableLayoutRowStyleCollection styles)
        {
            var source = (WebNavigationParameter)GetMain.cboWebsite.SelectedItem;
            var mapper = RowStyleChangeProvider.RowChangeProviders
                .FirstOrDefault(r => r.WebsiteIndex == source.Id);
            if (mapper == null) { return; }
            mapper.MapLabels(GetMain);
        }
    }
}
