using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FormDentonSetting : Form
    {
        public FormDentonSetting()
        {
            InitializeComponent();
            // cbo case search

            var countyCaseTypes
                = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.DentonCountyCaseType); // "dentonCountyCaseType");
            var districtCaseTypes
                = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.DentonDistrictCaseType); // "dentonDistrictCaseType");


            cboCaseSearchType.DataSource = countyCaseTypes.CaseSearchTypes;
            cboCaseSearchType.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboCaseSearchType.ValueMember = CommonKeyIndexes.IdProperCase;

            cboCourtListA.DataSource = countyCaseTypes.DropDowns.First().Options;
            cboCourtListA.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboCourtListA.ValueMember = CommonKeyIndexes.IdProperCase;

            cboCourtListB.DataSource = districtCaseTypes.DropDowns.First().Options;
            cboCourtListB.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboCourtListB.ValueMember = CommonKeyIndexes.IdProperCase;

            cboDistrictSearchType.DataSource = districtCaseTypes.CaseSearchTypes;
            cboDistrictSearchType.DisplayMember = CommonKeyIndexes.NameProperCase;
            cboDistrictSearchType.ValueMember = CommonKeyIndexes.IdProperCase;

            cboCaseSearchType.SelectedIndex = 0;
            const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
            var keys = Program.DentonCustomKeys;
            if (!keys.Any())
            {
                LoadFromSearchSettings();
                cboCaseSearchType.SelectedIndex = cboCaseSearchType.SelectedIndex;
                return;
            }
            var searchIndex = keys.Find(k => k.Name.Equals(
                CommonKeyIndexes.CaseSearchType, //"CaseSearchType"
                comparison));
            var searchTarget = countyCaseTypes.CaseSearchTypes.Find(x =>
                x.Query.Equals(searchIndex.Value, comparison));

            cboCaseSearchType.SelectedIndex = searchTarget.Id;
            var courtIndex = keys.Find(k => k.Name.Equals(
                CommonKeyIndexes.SearchComboIndex, // "SearchComboIndex" 
                comparison));
            var countIndexId = Convert.ToInt32(courtIndex.Value);
            var showDistrict = ((CaseSearchType)cboCaseSearchType.SelectedItem)
                .Name.Equals(CommonKeyIndexes.DistrictCourts, comparison);
            cboCourtListA.SelectedIndex = showDistrict ? 0 : countIndexId;
            cboCourtListB.SelectedIndex = showDistrict ? countIndexId : 0;
            cboDistrictSearchType.SelectedIndex = 0;
            if (!showDistrict)
            {
                return;
            }

            var districtIndex = keys.Find(k => k.Name.Equals(CommonKeyIndexes.DistrictSearchType, comparison));
            var districtTarget = districtCaseTypes.CaseSearchTypes.Find(x =>
                x.Query.Equals(districtIndex.Value, comparison));
            cboDistrictSearchType.SelectedIndex = districtTarget.Id;
            cboCaseSearchType.SelectedIndex = cboCaseSearchType.SelectedIndex;
        }

        private void LoadFromSearchSettings()
        {
            var settings = SearchSettingDto.GetDto();
            cboCaseSearchType.SelectedIndex = settings.CountySearchTypeId;
            cboDistrictSearchType.SelectedIndex = settings.DistrictSearchTypeId;
            cboCourtListA.SelectedIndex = settings.CountyCourtId;
            cboCourtListB.SelectedIndex = settings.DistrictCourtId;
            Save(false);
        }

        private void CboCaseSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
            var showDistrict = ((CaseSearchType)cboCaseSearchType.SelectedItem)
                .Name.Equals(CommonKeyIndexes.DistrictCourts, comparison);
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                if (control.Tag == null)
                {
                    continue;
                }

                control.Enabled = showDistrict ?
                    control.Tag.ToString().Contains(CommonKeyIndexes.DistrictKeyWord) :
                    control.Tag.ToString().Contains(CommonKeyIndexes.JpKeyword);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Save();
            DialogResult = DialogResult.OK;
        }

        private void Save(bool writeFile = true)
        {
            var showDistrict = ((CaseSearchType)cboCaseSearchType.SelectedItem)
                .Name.Equals(CommonKeyIndexes.DistrictCourts, // "District Courts", 
                StringComparison.CurrentCultureIgnoreCase);
            var caseSearchItem = (CaseSearchType)cboCaseSearchType.SelectedItem;
            var jpCourtItem = (Option)cboCourtListA.SelectedItem;
            var districtCourtItem = (Option)cboCourtListB.SelectedItem;
            var districtSearchItem = (CaseSearchType)cboDistrictSearchType.SelectedItem;
            var keyZero = new WebNavigationKey
            {
                Name = CommonKeyIndexes.SearchComboIndex, // "SearchComboIndex",
                Value = showDistrict ?
                (districtCourtItem.Id - 1).ToString() :
                (jpCourtItem.Id - 1).ToString()
            };
            var caseSearch = new WebNavigationKey
            {
                Name = CommonKeyIndexes.CaseSearchType, // "CaseSearchType",
                Value = caseSearchItem.Query
            };
            var keys = new List<WebNavigationKey>()
            {
                keyZero,
                caseSearch
            };
            if (showDistrict)
            {
                keys.Add(new WebNavigationKey
                {
                    Name = CommonKeyIndexes.DistrictSearchType, // "DistrictSearchType",
                    Value = districtSearchItem.Query
                });
            }
            Program.DentonCustomKeys = keys;
            if (!writeFile)
            {
                return;
            }
            // save settings to file
            var settings = new SearchSettingDto
            {
                CountySearchTypeId = showDistrict ? 1 : 0,
                CountyCourtId = jpCourtItem.Id - 1,
                DistrictCourtId = districtCourtItem.Id - 1,
                DistrictSearchTypeId = districtSearchItem.Id
            };
            SearchSettingDto.Save(settings);
        }

        public void Save()
        {
            Save(true);
        }
    }
}
