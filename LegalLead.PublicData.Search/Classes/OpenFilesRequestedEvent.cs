using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Classes
{
    public class OpenFilesRequestedEvent : PreviewSearchRequestedEvent
    {
        public override string Name => @"OpenFiles";
        public override void Toggle(bool isPreview, SearchResult context = null)
        {
            var manager = new OpenFileViewManager(GetPanel());
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
        protected class OpenFileViewManager(Panel panel) : PanelManager(panel, null)
        {
            public override void Populate()
            {
                Unload();
                var frm = new FormFileViewer { TopLevel = false };
                viewPanel.Controls.Add(frm);
                frm.FormBorderStyle = FormBorderStyle.None;
                frm.Dock = DockStyle.Fill;
                frm.Show();
            }
        }
    }
}