using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Classes
{
    public class OpenFilesRequestedEvent : PreviewSearchRequestedEvent
    {
        public OpenFilesRequestedEvent()
        {

        }
        public OpenFilesRequestedEvent(List<FileInfo> collection)
        {
            _collection = collection;
        }
        private readonly List<FileInfo> _collection = null;
        public override string Name => @"OpenFiles";
        public override void Toggle(bool isPreview, SearchResult context = null)
        {
            var manager = new OpenFileViewManager(GetPanel());
            if (isPreview)
            {
                Change();
                manager.RenderForm(_collection);
            }
            else
            {
                Reset();
            }
        }
        protected class OpenFileViewManager(Panel panel) : PanelManager(panel, null)
        {
            public void RenderForm(List<FileInfo> collection = null)
            {
                Unload();
                var frm = new FormFileViewer(collection) { TopLevel = false };
                viewPanel.Controls.Add(frm);
                frm.FormBorderStyle = FormBorderStyle.None;
                frm.Dock = DockStyle.Fill;
                frm.Show();
            }
        }
    }
}