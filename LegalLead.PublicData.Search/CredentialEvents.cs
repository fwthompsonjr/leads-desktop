// CredentialEvents
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
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
            var caseTypes = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.CollinCountyCaseType);
            var tarrantCourt = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.TarrantCountyCaseType); //"tarrantCountyCaseType");
            cboWebsite.DataSource = websites;
            cboWebsite.DisplayMember = CommonKeyIndexes.NameProperCase;// "Name";
            cboWebsite.ValueMember = CommonKeyIndexes.IdProperCase;

            cboWebsite.SelectedValueChanged += CboWebsite_SelectedValueChanged;
            cboWebsite.SelectedIndex = 1;
            cboWebsite.SelectedIndex = 2;
            BindPasswords();

        }

        private void BindPasswords()
        {
            var pwordlist = new List<UserPassword>();
            var history = UserAccessDto.GetListDto(CommonKeyIndexes.CollinCountyUserMap); // "collinCountyUserMap");
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
                Date = x.CreatedDate.GetValueOrDefault().ToString(
                    CommonKeyIndexes.DateTimeShort, //"MM/dd/yyyy", 
                CultureInfo.CurrentCulture),
                UserId = credential[0],
                Password = credential[1]
            });
        }

        private void CboWebsite_SelectedValueChanged(object sender, EventArgs e)
        {
            var source = (WebNavigationParameter)cboWebsite.SelectedItem;
            var isCollinCounty = source.Id == 20;
            var firstRw = 0;
            var lastRw = tableLayoutPanel1.RowStyles.Count - 1;
            var rows = tableLayoutPanel1.RowStyles;
            for (int i = firstRw; i <= lastRw; i++)
            {
                rows[i].SizeType = SizeType.Absolute;
                rows[i].Height = isCollinCounty ? 50 : 0;
                // alter collin collin style
                if (isCollinCounty)
                {
                    rows[i].Height = rows[i].Height;
                }
            }
            var tbx = new List<Control> { tbxPwd, tbxUser };
            tbx.ForEach(v => v.Visible = isCollinCounty);
            tbx.ForEach(v => v.Text = string.Empty);
            if (!isCollinCounty)
            {
                return;
            }

            var dto = UserAccessDto.GetDto(CommonKeyIndexes.CollinCountyUserMap);
            var parts = UserAccessDto.GetCredential(dto);
            tbx[0].Text = parts[0];
            tbx[1].Text = parts[1];
        }

        protected void ChangePassword()
        {
            var dto = UserAccessDto.GetDto(CommonKeyIndexes.CollinCountyUserMap);
            var cleared = string.Format(CultureInfo.CurrentCulture,
                CommonKeyIndexes.ElementPipeElement, tbxUser.Text, tbxPwd.Text);
            UserAccessDto.CreateCredential(cleared, dto.UserKey, CommonKeyIndexes.CollinCountyUserMap);
            BindPasswords();
            DialogResult = DialogResult.OK;
        }



    }
}
