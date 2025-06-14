﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FsMyProfile : Form
    {
        public FsMyProfile()
        {
            InitializeComponent();
            tbxUserName.Text = GetUserName();
            PopulateComboBox();
            cboProfileGroup.SelectedIndexChanged += CboProfileGroup_SelectedIndexChanged;
            CboProfileGroup_SelectedIndexChanged(null, null);
            var textboxes = tableLayout.Controls.Cast<Control>()
                .Where(w => w.GetType() == typeof(TextBox))
                .Cast<TextBox>()
                .Where(w => w.Tag != null)
                .Where(w => (Convert.ToString(w.Tag) ?? "").StartsWith("FieldValue"))
                .ToList();
            textboxes.ForEach(w =>
            {
                w.Leave += FieldValue_TextChanged;
            });
            var nonfocused = new List<TextBox> {
                tbxUserName,
                tbxFieldName01,
                tbxFieldName02,
                tbxFieldName03,
                txErrorMessages
            };
            nonfocused.ForEach(tbx =>
            {
                tbx.Enter += TextBox_Entered;
                tbx.MouseDown += TextBox_MouseEntered;
            });
        }

        private void TextBox_MouseEntered(object sender, MouseEventArgs e)
        {
            if (sender is not TextBox tbx) return;
            var name = tbx.Name;
            Control target = name switch
            {
                "tbxUserName" => cboProfileGroup,
                "tbxFieldName01" => tbxFieldValue01,
                "tbxFieldName02" => tbxFieldValue02,
                "tbxFieldName03" => tbxFieldValue03,
                "txErrorMessages" => button1,
                _ => null,
            };
            if(target == null) return;
            ActiveControl = target;
        }

        private void TextBox_Entered(object sender, EventArgs e)
        {
            TextBox_MouseEntered(sender, null);
        }

        private void FieldValue_TextChanged(object sender, EventArgs e)
        {
            if (sender is not TextBox tbx) return;
            if (tbx.Tag is not string txtag) return;
            if (!txtag.StartsWith("FieldValue")) return;
            if (Tag is not string json) return;
            var list = json.ToInstance<List<UserProfileModel>>();
            if (list == null) return;
            var id = txtag.Split('_')[^1];
            if (!int.TryParse(id, out var indx)) return;
            if (indx > list.Count - 1) return;
            var target = list[indx];
            target.KeyValue = tbx.Text;
            var change = changes.Find(x => x.Id == target.Id);
            if (change == null)
            {
                changes.Add(target);
                return;
            }
            change.KeyValue = target.KeyValue;
        }

        private void CboProfileGroup_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var groupName = cboProfileGroup.SelectedItem as string;
            BindTextBoxes(groupName);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var canSubmit = ValidateUserInput();
            if(!canSubmit) return;
            if (cboProfileGroup.SelectedItem is not string groupName) return;
            var db = changes.FindAll(x => x.ProfileGroup == groupName);
            var response = dbHelper.UpdateMyProfile(db);
            if (response == null || response.Count == 0) return;
            txErrorMessages.Clear();
            txErrorMessages.Text = $"Successfully updated {db.Count} records";
            data.Clear();
            data.AddRange(response);
            var indexes = db.Select(x => x.Id);
            changes.RemoveAll(x => indexes.Contains(x.Id));
            CboProfileGroup_SelectedIndexChanged(null, null);
        }


    }
}
