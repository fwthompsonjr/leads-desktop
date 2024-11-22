using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FormMain : Form
    {

        private void ApplySavedSettings()
        {
            var handler = new LastUserSearchHandler();
            handler.ApplyWebSite(cboWebsite);
            handler.ApplyDateTime(dteStart, 0);
            handler.ApplyDateTime(dteEnding, 1);
        }


        private sealed class LastUserSearchHandler
        {
            public LastUserSearchHandler()
            {
                DateTime? fallbackDt = null;
                _websiteName = SettingsWriter.GetSettingOrDefault("search", "Last County:", string.Empty);
                _startingDate = SettingsWriter.GetSettingOrDefault("search", "Start Date:", fallbackDt);
                _endingDate = SettingsWriter.GetSettingOrDefault("search", "End Date:", fallbackDt);
            }

            private readonly string _websiteName;
            private readonly DateTime? _startingDate;
            private readonly DateTime? _endingDate;

            public void ApplyWebSite(ComboBox comboBox)
            {
                if (string.IsNullOrEmpty(_websiteName)) return;
                if (comboBox.DataSource is not List<WebNavigationParameter> websites) return;
                var requested = websites.Find(x => x.Name == _websiteName);
                if (requested == null) return;
                comboBox.SelectedIndex = websites.IndexOf(requested);
            }
            public void ApplyDateTime(DateTimePicker dateBox, int typeId = 0)
            {
                var targets = new[] { _startingDate, _endingDate };
                if (typeId < 0 || typeId > targets.Length - 1) return;
                var targetDt = targets[typeId];
                if (!targetDt.HasValue) return;
                dateBox.Value = targetDt.Value;
            }
        }
    }
}