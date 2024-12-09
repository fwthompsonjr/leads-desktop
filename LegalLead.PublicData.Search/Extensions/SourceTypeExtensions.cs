using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Models;
using System;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Extensions
{
    internal static class SourceTypeExtensions
    {
        public static string GetCountyName(this SourceType source)
        {
            var name = Enum.GetName(source);
            name = name.Replace("County", string.Empty);
            name = name.Replace("Civil", string.Empty);
            name = name.Replace("Criminal", string.Empty);
            name = name.Replace("ElPaso", "El Paso");
            name = name.Replace("FortBend", "Fort Bend");
            return name;
        }

        public static FindDbRequest GetDbRequest(
            this SourceType source,
            FormMain form)
        {
            var cboCT = form.cboCaseType;
            var model = new FindDbRequest
            {
                CountyId = (int)source,
                CountyName = GetCountyName(source),
                CourtTypeName = GetCaseTypeName(cboCT),
                SearchTypeId = cboCT.SelectedIndex,
                CaseTypeId = cboCT.SelectedIndex

            };
            return model;
        }

        private static string GetCaseTypeName(ComboBoxEx comboBox)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            if (!comboBox.Visible) return string.Empty;
            if (comboBox.SelectedItem is not DropDown drop) return string.Empty;
            var displayText = drop.Name;
            if (!displayText.Contains(' ')) return displayText;
            if (displayText.Contains("COUNTY", oic)) return "COUNTY";
            if (displayText.Contains("JUSTICE", oic)) return "JUSTICE";
            if (displayText.Contains("DISTRICT", oic)) return "DISTRICT";
            if (displayText.Contains("CIVIL", oic)) return "CIVIL";
            if (displayText.Contains("CRIMINAL", oic)) return "CRIMINAL";
            if (displayText.Contains("PROBATE", oic)) return "PROBATE";
            if (displayText.Contains("MAGISTRATE", oic)) return "MAGISTRATE";
            return string.Empty;
        }
    }
}
