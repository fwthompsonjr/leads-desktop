using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    fschange.FormBorderStyle = FormBorderStyle.None;
                    fschange.Dock = DockStyle.Fill;
                    fschange.Show();
                    break;
                default:
                    break;
            }
        }

        private sealed class SettingModel {
            public int Id { get; set; } = 0;
            public string Name { get; set; } = string.Empty;
        }

        private static readonly List<SettingModel> options
            = new()
            {
                new SettingModel() { Id = 0, Name = "Change Password" }
            };
    }
}
