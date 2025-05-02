using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Classes
{
    public class ViewOfflineRequestsEvent : PreviewSearchRequestedEvent
    {
        public override string Name => @"ViewOfflineRequests";
        public override void Toggle(bool isPreview, SearchResult context = null)
        {
            var manager = new OfflineRequestManager(GetPanel());
            if (isPreview)
            {
                Change();
                manager.Populate();
            }
            else
            {
                Reset();
            }
        }
        protected class OfflineRequestManager(Panel panel) : PanelManager(panel, null)
        {
            public override void Populate()
            {
                Unload();
                var frm = new FsOfflineHistory { TopLevel = false };
                viewPanel.Controls.Add(frm);
                frm.FormBorderStyle = FormBorderStyle.None;
                frm.Dock = DockStyle.Fill;
                frm.Show();
            }
        }
    }
}
