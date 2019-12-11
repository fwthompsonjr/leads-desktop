// CredentialEvents
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FormCredential : Form
    {
        protected class UserPassword
        {
            public string Date { get; set; }
            public string UserId { get; set; }
            public string Password { get; set; }
        }

        private void BindComboBoxes()
        {

            var websites = SettingsManager.GetNavigation();
            var caseTypes = CaseTypeSelectionDto.GetDto(Thompson.RecordSearch.Utility.CommonKeyIndexes.CollinCountyCaseType);
            var tarrantCourt = CaseTypeSelectionDto.GetDto("tarrantCountyCaseType");
            cboWebsite.DataSource = websites;
            cboWebsite.DisplayMember = "Name";
            cboWebsite.ValueMember = "Id";

            cboWebsite.SelectedValueChanged += CboWebsite_SelectedValueChanged;
            cboWebsite.SelectedIndex = 1;
            cboWebsite.SelectedIndex = 2;
            BindPasswords();

        }

        private void BindPasswords()
        {
            var pwordlist = new List<UserPassword>();
            var history = UserAccessDto.GetListDto("collinCountyUserMap");
            history.ForEach(x => Append(pwordlist, x));
            var bs = new BindingSource
            {
                DataSource = pwordlist
            };
            dataGridView1.DataSource = bs;
        }

        private static void Append(List<UserPassword> pwordlist, UserAccessDto x)
        {
            var credential = UserAccessDto.GetCredential(x);
            pwordlist.Add(new UserPassword
            {
                Date = x.CreatedDate.GetValueOrDefault().ToString("MM/dd/yyyy", CultureInfo.CurrentCulture),
                UserId = credential[0],
                Password = credential[1]
            });
        }

        private void CboWebsite_SelectedValueChanged(object sender, EventArgs e)
        {
            var source = (WebNavigationParameter)cboWebsite.SelectedItem;
            var isCollinCounty = source.Id == 20;
            var firstRw = 1;
            var lastRw = 4;
            for (int i = firstRw; i <= lastRw; i++)
            {
                tableLayoutPanel1.RowStyles[i].SizeType = SizeType.Absolute;
                tableLayoutPanel1.RowStyles[i].Height = isCollinCounty ? 50 : 0;
            }
            var tbx = new List<Control> { tbxPwd, tbxUser };
            tbx.ForEach(v => v.Visible = isCollinCounty);
            tbx.ForEach(v => v.Text = string.Empty);
            if (!isCollinCounty) return;
            var dto = UserAccessDto.GetDto("collinCountyUserMap");
            var parts = UserAccessDto.GetCredential(dto);
            tbx[0].Text = parts[0];
            tbx[1].Text = parts[1];
        }

        protected void ChangePassword()
        {
            var dto = UserAccessDto.GetDto("collinCountyUserMap");
            var cleared = string.Format(CultureInfo.CurrentCulture, @"{0}|{1}", tbxUser.Text, tbxPwd.Text);
            UserAccessDto.CreateCredential(cleared, dto.UserKey, "collinCountyUserMap");
            BindPasswords();
            DialogResult = DialogResult.OK;
        }



    }
}
