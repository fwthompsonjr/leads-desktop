using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FormSettings : Form
    {
        public FormSettings(int menuId = 0, bool isAdmin = false)
        {
            InitializeComponent();
            IsMdiContainer = true;
            var options = GetMenu(isAdmin);
            cboSelection.DataSource = options;
            cboSelection.DisplayMember = "Name";
            cboSelection.ValueMember = "Id";
            cboSelection.SelectedIndexChanged += CboSelection_SelectedIndexChanged;
            if (options.Exists(x => x.Id == menuId))
            {
                cboSelection.SelectedIndex = menuId;
            }
            CboSelection_SelectedIndexChanged(null, null);
        }

        private void CboSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var item in panel1.Controls)
            {
                if (item is Form frm) { frm.Close(); }
            }
            panel1.Controls.Clear();
            if (cboSelection.SelectedItem is not SettingMenuModel model)
            {
                return;
            }
            Form form = model.Id switch
            {
                0 => new FsChangePassword { TopLevel = false },
                1 => new FsCountySetting { TopLevel = false },
                2 => new FsUserSettings { TopLevel = false },
                3 => new FsSearchHistory { TopLevel = false },
                4 => new FsInvoiceHistory { TopLevel = false },
                5 => new FsOfflineHistory { TopLevel = false },
                _ => null
            };
            if (form == null) return;
            panel1.Controls.Add(form);
            RenderChild(form);
        }
        private static void RenderChild(Form fschange)
        {
            fschange.FormBorderStyle = FormBorderStyle.None;
            fschange.Dock = DockStyle.Fill;
            fschange.Show();
        }

        internal void SetMenuIndex(int id, bool isAdmin = false)
        {
            var requested = GetMenu(isAdmin).Find(x => x.Id == id);
            if (requested == null) return;
            cboSelection.SelectedIndex = requested.SelectedIndex;
            CboSelection_SelectedIndexChanged(null, null);
        }

        private static List<SettingMenuItem> GetMenu(bool isAdmin)
        {
            var options = SessionUtil.GetMenuOptions.FindAll(x => isAdmin || !x.Name.Equals("County Permissions"));
            var items = options.Select(x => new SettingMenuItem { Name = x.Name, Id = x.Id, SelectedIndex = options.IndexOf(x) });
            return [ ..items];
        }

        private sealed class SettingMenuItem : SettingMenuModel
        {
            public int SelectedIndex { get; set; } = 0;
        }
    }
}
