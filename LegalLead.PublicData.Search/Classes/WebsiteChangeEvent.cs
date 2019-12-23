using System.Collections.Generic;
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
                source.Id == (int)SourceType.CollinCounty);
            GetMain.cboSearchType.Visible = source.Id == (int)SourceType.CollinCounty;
            GetMain.cboCaseType.Visible = source.Id == (int)SourceType.CollinCounty;
            GetMain.cboCourts.Visible = source.Id == (int)SourceType.TarrantCounty;
            var fives = new List<int>
            {
                (int)SourceType.CollinCounty,
                (int)SourceType.TarrantCounty
            };
            for (int i = Three; i <= Five; i++)
            {
                GetMain.tableLayoutPanel1.RowStyles[i].SizeType = SizeType.Absolute;
                GetMain.tableLayoutPanel1.RowStyles[i].Height = fives.Contains(source.Id) 
                    ? FortyNine : Zero;
                if (i == Five)
                {
                    GetMain.tableLayoutPanel1.RowStyles[i].Height = fives.Contains(source.Id) 
                        ? FortyNine : Zero;
                }
            }
            GetMain.tsStatusLabel.Text = string.Empty;
            // when in Denton County write Settings
            if (source.Id == (int)SourceType.DentonCounty)
            {
                GetMain.ButtonDentonSetting.Text = CommonKeyIndexes.SettingsLabel; // "Settings";
                GetMain.SetDentonStatusLabelFromSetting();
            }
            else
            {
                GetMain.ButtonDentonSetting.Text = CommonKeyIndexes.PasswordLabel; // "Password";
            }
        }
    }
}
