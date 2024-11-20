using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static System.Windows.Forms.Design.AxImporter;

namespace LegalLead.PublicData.Search
{
    public partial class FormSettings : Form
    {
        public FormSettings(int menuId = 0)
        {
            InitializeComponent();
            IsMdiContainer = true;
            var options = SessionUtil.GetMenuOptions;
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

        internal void SetMenuIndex(int id)
        {
            if (!SessionUtil.GetMenuOptions.Exists(x => x.Id == id)) return;
            cboSelection.SelectedIndex = id;
            CboSelection_SelectedIndexChanged(null, null);
        }
    }
}
