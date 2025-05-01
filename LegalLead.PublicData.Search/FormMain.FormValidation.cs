using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FormMain : Form
    {

        private bool ValidateCustom()
        {
            if (cboWebsite.SelectedIndex < 0)
            {
                MessageBox.Show(CommonKeyIndexes.PleaseChooseWebsite,
                    CommonKeyIndexes.DataValidationError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            if (dteStart.Value.Date > dteEnding.Value.Date)
            {
                MessageBox.Show(
                    CommonKeyIndexes.PleaseCheckStartAndEndDates,
                    CommonKeyIndexes.DataValidationError,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
            var siteData = (WebNavigationParameter)(cboWebsite.SelectedItem);
            var dateRange = siteData.Keys.FirstOrDefault(x => x.Name.Equals(
                CommonKeyIndexes.DateRangeMaxDays, comparison));
            if (dateRange != null)
            {
                int maxDayInterval = GetMaxDateRangeSetting();
                var businessDays = DallasSearchProcess.GetBusinessDays(dteStart.Value.Date, dteEnding.Value.Date).Count; ;
                if (businessDays > maxDayInterval)
                {
                    MessageBox.Show(CommonKeyIndexes.PleaseCheckStartEndRange +
                        Environment.NewLine +
                        CommonKeyIndexes.StartDateToEndDateRange +
                        Environment.NewLine +
                        string.Format(CultureInfo.CurrentCulture, CommonKeyIndexes.RangeExceedsMaximunDays,
                        maxDayInterval),
                        CommonKeyIndexes.DataValidationError,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
            }
            if (!ValidateCustomDenton(siteData))
            {
                return false;
            }

            if (!ValidateCustomCollin(siteData))
            {
                return false;
            }

            if (!ValidateCustomTarrant(siteData))
            {
                return false;
            }

            if (!ValidateCustomHarrisCivil(siteData))
            {
                return false;
            }

            return true;
        }

        private bool ValidateCustomDenton(WebNavigationParameter siteData)
        {
            const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
            if (siteData.Id != 1)
            {
                return true;
            }

            var keys = Program.DentonCustomKeys;
            if (!keys.Any())
            {
                return true;
            }

            foreach (var customKey in keys)
            {
                var found = siteData.Keys.FirstOrDefault(k => k.Name.Equals(customKey.Name, comparison));
                if (found != null)
                {
                    found.Value = customKey.Value;
                }
                else
                {
                    siteData.Keys.Add(customKey);
                }
            }
            var isDistrictSearch = tsStatusLabel.Text.Contains(CommonKeyIndexes.DistrictKeyWord);
            if (!isDistrictSearch)
            {
                // remove district item from keys collection
                var districtItem = keys.FirstOrDefault(x => x.Name.Equals(
                    CommonKeyIndexes.DistrictSearchType, // "DistrictSearchType", 
                    comparison));
                if (districtItem != null)
                {
                    keys.Remove(districtItem);
                }
            }
            return true;
        }

        private bool ValidateCustomCollin(WebNavigationParameter siteData)
        {

            if (siteData.Id != 20)
            {
                return true;
            }

            if (cboCaseType.SelectedIndex < 0)
            {
                MessageBox.Show(CommonKeyIndexes.PleaseChooseValidCaseType,
                    CommonKeyIndexes.DataValidationError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            else
            {
                // update site data with correct index
                // SetKeyValue("startDate", startDate.ToString("MM/dd/yyyy"));
                SetKeyValue(siteData, CommonKeyIndexes.CaseTypeSelectedIndex,
                Convert.ToInt32(cboCaseType.SelectedValue,
                    CultureInfo.CurrentCulture.NumberFormat)
                    .ToString("0", CultureInfo.CurrentCulture.NumberFormat));
            }
            if (cboSearchType.SelectedIndex < 0)
            {
                MessageBox.Show(CommonKeyIndexes.PleaseChooseValidCaseSearchType,
                    CommonKeyIndexes.DataValidationError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            else
            {
                // update site data with correct index
                // SetKeyValue("startDate", startDate.ToString("MM/dd/yyyy"));
                SetKeyValue(siteData, CommonKeyIndexes.SearchTypeSelectedIndex, // "searchTypeSelectedIndex",
                Convert.ToInt32(cboSearchType.SelectedValue,
                    CultureInfo.CurrentCulture.NumberFormat)
                    .ToString("0", CultureInfo.CurrentCulture.NumberFormat));
            }
            return true;
        }

        private bool ValidateCustomTarrant(WebNavigationParameter siteData)
        {
            if (siteData.Id != 10)
            {
                return true;
            }

            if (cboCourts.SelectedIndex < 0)
            {
                MessageBox.Show(CommonKeyIndexes.PleaseChooseValidCourt,
                    CommonKeyIndexes.DataValidationError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                // update site data with correct index
                // SetKeyValue("startDate", startDate.ToString("MM/dd/yyyy"));
                SetKeyValue(siteData, CommonKeyIndexes.CaseTypeSelectedIndex,
                Convert.ToInt32(cboCourts.SelectedValue
                    , CultureInfo.CurrentCulture.NumberFormat)
                    .ToString("0", CultureInfo.CurrentCulture.NumberFormat));

                SetKeyValue(siteData, CommonKeyIndexes.CriminalCaseInclusion,
                    cboCaseType.SelectedValue.ToString());
            }
            return true;
        }


        private bool ValidateCustomHarrisCivil(WebNavigationParameter siteData)
        {
            const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
            if (siteData.Id != (int)SourceType.HarrisCivil)
            {
                return true;
            }

            var court = ((Thompson.RecordSearch.Utility.Dto.Option)(cboCaseType.SelectedItem));
            var caseStatus = ((Thompson.RecordSearch.Utility.Dto.DropDown)(cboSearchType.SelectedItem));
            var keys = new List<WebNavigationKey>() {
                new() {
                    Name = "courtIndex",
                    Value = court.Id.ToString() },
                new() {
                    Name= "caseStatusIndex",
                    Value = caseStatus.Id.ToString()}
            };
            if (!keys.Any())
            {
                return true;
            }

            foreach (var customKey in keys)
            {
                var found = siteData.Keys.FirstOrDefault(k => k.Name.Equals(customKey.Name, comparison));
                if (found != null)
                {
                    found.Value = customKey.Value;
                }
                else
                {
                    siteData.Keys.Add(customKey);
                }
            }
            return true;
        }

        private int GetMaxDateRangeSetting()
        {
            var fallbak = GetMaxDateFallback();
            if (!IsAccountAdmin()) return fallbak;
            const string findDate = SettingConstants.AdminFieldNames.ExtendedDateMaxDays;
            var changeModel = UserDataReader.GetList<UserSettingChangeModel>();
            var model = changeModel.Find(x => x.Index == 16 && x.Name == findDate);
            if (model == null || !int.TryParse(model.Value, out var number)) return fallbak;
            return number;
        }
        private int GetMaxDateFallback()
        {
            const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
            var siteData = (WebNavigationParameter)(cboWebsite.SelectedItem);
            var dateRange = siteData.Keys.FirstOrDefault(x => x.Name.Equals(
                CommonKeyIndexes.DateRangeMaxDays, comparison));
            if (dateRange == null || !int.TryParse(dateRange.Value, out var number)) return 7;
            return number;
        }
        private static void SetKeyValue(WebNavigationParameter siteData,
            string keyName,
            string keyValue)
        {
            var keys = siteData.Keys;
            var item = keys.First(k => k.Name.Equals(keyName, StringComparison.CurrentCultureIgnoreCase));
            if (item == null)
            {
                return;
            }

            item.Value = keyValue;

        }


        private static readonly SessionSettingPersistence UserDataReader =
            SessionPersistenceContainer
            .GetContainer
            .GetInstance<SessionSettingPersistence>();

    }
}
