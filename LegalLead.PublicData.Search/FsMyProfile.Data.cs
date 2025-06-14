using System;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Models;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search
{
    public partial class FsMyProfile
    {
        private readonly List<UserProfileModel> data = new();
        private readonly List<UserProfileModel> changes = new();

        private void PopulateData()
        {
            data.Clear();
            data.AddRange(GetData());
        }

        private void PopulateComboBox()
        {
            if (data.Count == 0) PopulateData();
            var items = data.Select(x => x.ProfileGroup).Distinct().ToList();
            cboProfileGroup.Items.Clear();
            cboProfileGroup.DataSource = items;
        }
        private void BindTextBoxes(string profileGroup)
        {
            var textboxes = tableLayout.Controls.Cast<Control>()
                .Where(w => w.GetType() == typeof(TextBox))
                .Cast<TextBox>()
                .Where(w => w.Tag != null).ToList();
            textboxes.ForEach(t => t.Text = string.Empty);
            var context = data.FindAll(x => x.ProfileGroup.Equals(profileGroup));
            var serialized = context.ToJsonString();
            Tag = serialized;
            var count = context.Count - 1;
            for (var i = 0; i < 3; i++) {
                if (i > count) return;
                var item = context[i];
                var tbxField = textboxes.Find(x => Convert.ToString(x.Tag) == $"FieldName_{i:D2}");
                var tbxValue = textboxes.Find(x => Convert.ToString(x.Tag) == $"FieldValue_{i:D2}");
                if (tbxField != null ) tbxField.Text = item.KeyName;
                if (tbxValue != null) tbxValue.Text = item.KeyValue ?? string.Empty;
            }

        }
        private static string GetUserName()
        {
            var container = AuthenicationContainer.GetContainer;
            var userservice = container.GetInstance<SessionUserPersistence>();
            return userservice.GetUserName();
        }

        private static List<UserProfileModel> GetData()
        {
            var list = dbHelper.GetMyProfile();
            return list;
        }
        private static readonly IRemoteDbHelper dbHelper
                = ActionSettingContainer.GetContainer.GetInstance<IRemoteDbHelper>();
    }
}