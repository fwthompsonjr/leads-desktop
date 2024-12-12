using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
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
            FormMain form,
            DateTime startDate)
        {
            var cboST = form.cboSearchType;
            var model = new FindDbRequest
            {
                CountyId = (int)source,
                SearchDate = startDate,
                CountyName = GetCountyName(source),
                CourtTypeName = GetCaseTypeName(cboST),
                SearchTypeId = cboST.SelectedIndex
            };
            if (Common.Contains(source)) return model;
            if (source == SourceType.CollinCounty)
            {
                model.CaseTypeId = form.cboCourts.SelectedIndex;
                return model;
            }
            if (source == SourceType.DentonCounty)
            {
                var settings = SearchSettingDto.GetDto();
                model.SearchTypeId = settings.CountySearchTypeId;
                model.CaseTypeId = settings.CountyCourtId;
                model.DistrictCourtId = settings.DistrictCourtId;
                model.DistrictSearchTypeId = settings.DistrictSearchTypeId;
                return model;
            }
            if (source == SourceType.HarrisCivil)
            {
                model.CaseTypeId = form.cboCaseType.SelectedIndex;
                model.CourtTypeName = model.CaseTypeId == 0 ? "CRIMINAL" : "CIVIL";
                return model;
            }
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
        private static readonly List<SourceType> Common = new()
        {
            SourceType.BexarCounty,
            SourceType.DallasCounty,
            SourceType.ElPasoCounty,
            SourceType.FortBendCounty,
            SourceType.GraysonCounty,
            SourceType.HidalgoCounty,
            SourceType.TravisCounty,
            SourceType.WilliamsonCounty,
        };
    }
}
