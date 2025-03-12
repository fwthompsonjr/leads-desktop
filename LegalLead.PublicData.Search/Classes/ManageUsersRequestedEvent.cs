using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Classes
{
    public class ManageUsersRequestedEvent : PreviewSearchRequestedEvent
    {
        public override string Name => @"ManageUsers";
        public override void Toggle(bool isPreview, SearchResult context = null)
        {
            var manager = new AdminUserViewManager(GetPanel());
            if (isPreview)
            {
                Change();
                manager.RenderForm();
            }
            else
            {
                Reset();
            }
        }
        protected class AdminUserViewManager(Panel panel) : PanelManager(panel, null)
        {
            public void RenderForm()
            {
                Unload();
                var frm = new FormAdmin() { TopLevel = false };
                viewPanel.Controls.Add(frm);
                frm.FormBorderStyle = FormBorderStyle.None;
                frm.Dock = DockStyle.Fill;
                frm.Show();
            }
        }
    }
}