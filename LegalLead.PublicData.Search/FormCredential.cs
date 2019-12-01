using System;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FormCredential : Form
    {
        public FormCredential()
        {
            InitializeComponent();
            BindComboBoxes();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            ChangePassword();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
