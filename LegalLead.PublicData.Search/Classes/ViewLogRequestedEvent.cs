using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Classes
{
    public class ViewLogRequestedEvent : PreviewSearchRequestedEvent
    {
        public override string Name => @"ViewLog";
        public override void Toggle(bool isPreview, SearchResult context = null)
        {
            var manager = new LogViewManager(GetPanel());
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
        protected class LogViewManager(Panel panel) : PanelManager(panel, null)
        {
            public override void Populate()
            {
                Unload();
                var frm = new FormLogViewer { TopLevel = false };
                viewPanel.Controls.Add(frm);
                frm.FormBorderStyle = FormBorderStyle.None;
                frm.Dock = DockStyle.Fill;
                frm.Show();
            }
        }
    }
}
