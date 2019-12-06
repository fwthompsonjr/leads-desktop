using System;
using System.Linq;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FormMain : Form
    {

        private bool ValidateCustom()
        {
            if (cboWebsite.SelectedIndex < 0)
            {
                MessageBox.Show("Please choose a website",
                    "Data Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            if (dteStart.Value.Date > dteEnding.Value.Date)
            {
                MessageBox.Show(
                    "Please check start/end dates. Start date must be less than End Date.",
                    "Data Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
            var siteData = (WebNavigationParameter)(cboWebsite.SelectedItem);
            var dateRange = siteData.Keys.FirstOrDefault(x => x.Name.Equals("dateRangeMaxDays", comparison));
            if (dateRange != null)
            {
                int maxDayInterval = Convert.ToInt32(dateRange.Value);
                var dayInterval = Math.Abs(Convert.ToInt32(dteStart.Value.Date.Subtract(
                    dteEnding.Value.Date).TotalDays));
                if (dayInterval > maxDayInterval)
                {
                    MessageBox.Show("Please check start/end dates. " +
                        Environment.NewLine +
                        "Start date - End Date Date Range " +
                        Environment.NewLine +
                        string.Format("exceeds maximum of ({0}) days.", maxDayInterval),
                        "Data Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
            }
            if (!ValidateCustomDenton(siteData)) return false;
            if (!ValidateCustomCollin(siteData)) return false;
            if (!ValidateCustomTarrant(siteData)) return false;
            return true;
        }

        private bool ValidateCustomDenton(WebNavigationParameter siteData)
        {
            const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
            if (siteData.Id != 1) return true;
            var keys = Program.DentonCustomKeys;
            if (!keys.Any()) return true;
            foreach (var customKey in keys)
            {
                var found = siteData.Keys.FirstOrDefault(k => k.Name.Equals(customKey.Name, comparison));
                if(found != null)
                {
                    found.Value = customKey.Value;
                }
                else
                {
                    siteData.Keys.Add(customKey);
                }
            }
            var isDistrictSearch = tsStatusLabel.Text.Contains("District");
            if (!isDistrictSearch)
            {
                // remove district item from keys collection
                var districtItem = keys.FirstOrDefault(x => x.Name.Equals("DistrictSearchType", comparison));
                if(districtItem != null)
                {
                    keys.Remove(districtItem);
                }
            }
            return true;
        }

        private bool ValidateCustomCollin(WebNavigationParameter siteData)
        {

            if (siteData.Id != 20) return true;
            if (cboCaseType.SelectedIndex < 0)
            {
                MessageBox.Show("Please choose a valid case type",
                    "Data Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            else
            {
                // update site data with correct index
                // SetKeyValue("startDate", startDate.ToString("MM/dd/yyyy"));
                SetKeyValue(siteData, "caseTypeSelectedIndex",
                Convert.ToInt32(cboCaseType.SelectedValue).ToString("0"));
            }
            if (cboSearchType.SelectedIndex < 0)
            {
                MessageBox.Show("Please choose a valid case search type",
                    "Data Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            else
            {
                // update site data with correct index
                // SetKeyValue("startDate", startDate.ToString("MM/dd/yyyy"));
                SetKeyValue(siteData, "searchTypeSelectedIndex",
                Convert.ToInt32(cboSearchType.SelectedValue).ToString("0"));
            }
            return true;
        }

        private bool ValidateCustomTarrant(WebNavigationParameter siteData)
        {
            if (siteData.Id != 10) return true;
            if (cboCourts.SelectedIndex < 0)
            {
                MessageBox.Show("Please choose a valid court", "Data Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                // update site data with correct index
                // SetKeyValue("startDate", startDate.ToString("MM/dd/yyyy"));
                SetKeyValue(siteData, "caseTypeSelectedIndex",
                Convert.ToInt32(cboCourts.SelectedValue).ToString("0"));

                SetKeyValue(siteData, "criminalCaseInclusion", 
                    cboCaseType.SelectedValue.ToString());
            }
            return true;
        }


        private static void SetKeyValue(WebNavigationParameter siteData,
            string keyName,
            string keyValue)
        {
            var keys = siteData.Keys;
            var item = keys.First(k => k.Name.Equals(keyName, StringComparison.CurrentCultureIgnoreCase));
            if (item == null) return;
            item.Value = keyValue;

        }

    }
}
