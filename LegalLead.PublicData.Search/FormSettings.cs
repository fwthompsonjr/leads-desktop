using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
            IsMdiContainer = true;
            cboSelection.DataSource = options;
            cboSelection.DisplayMember = "Name";
            cboSelection.ValueMember = "Id";
            cboSelection.SelectedIndexChanged += CboSelection_SelectedIndexChanged;
            cboSelection.SelectedIndex = 0;
            CboSelection_SelectedIndexChanged(null, null);
        }

        private void CboSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var item in panel1.Controls)
            {
                if (item is Form frm) { frm.Close(); }
            }
            panel1.Controls.Clear();
            if (cboSelection.SelectedItem is not SettingModel model)
            {
                return;
            }
            switch (model.Id)
            {
                case 0:
                    var fschange = new FsChangePassword
                    {
                        TopLevel = false
                    };
                    panel1.Controls.Add(fschange);
                    RenderChild(fschange);
                    break;
                case 1:
                    var fscounty = new FsCountySetting
                    {
                        TopLevel = false
                    };
                    panel1.Controls.Add(fscounty);
                    RenderChild(fscounty);
                    break;
                default:
                    break;
            }
        }
        private static void RenderChild(Form fschange)
        {
            fschange.FormBorderStyle = FormBorderStyle.None;
            fschange.Dock = DockStyle.Fill;
            fschange.Show();
        }
        private sealed class SettingModel
        {
            public int Id { get; set; } = 0;
            public string Name { get; set; } = string.Empty;
        }

        private static readonly List<SettingModel> options
            = new()
            {
                new SettingModel() { Id = 0, Name = "Change Password" },
                new SettingModel() { Id = 1, Name = "County Permissions" }
            };
    }
}
