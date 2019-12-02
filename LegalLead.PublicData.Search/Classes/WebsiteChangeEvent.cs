using System.Windows.Forms;
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
            GetMain.ButtonDentonSetting.Visible = (source.Id == (int)SourceType.DentonCounty | source.Id == (int)SourceType.CollinCounty);
            GetMain.cboSearchType.Visible = source.Id == (int)SourceType.CollinCounty;
            GetMain.cboCaseType.Visible = source.Id == (int)SourceType.CollinCounty;
            GetMain.cboCourts.Visible = source.Id == (int)SourceType.TarrantCounty;
            for (int i = 3; i <= 5; i++)
            {
                GetMain.tableLayoutPanel1.RowStyles[i].SizeType = SizeType.Absolute;
                GetMain.tableLayoutPanel1.RowStyles[i].Height = source.Id == (int)SourceType.CollinCounty ? 49 : 0;
                if (i == 5)
                {
                    GetMain.tableLayoutPanel1.RowStyles[i].Height = source.Id == (int)SourceType.TarrantCounty ? 49 : 0;
                }
            }
            GetMain.tsStatusLabel.Text = string.Empty;
            // when in Denton County write Settings
            if (source.Id == (int)SourceType.DentonCounty)
            {
                GetMain.ButtonDentonSetting.Text = "Settings";
                GetMain.SetDentonStatusLabelFromSetting();
            }
            else
            {
                GetMain.ButtonDentonSetting.Text = "Password";
            }
        }
    }
}
