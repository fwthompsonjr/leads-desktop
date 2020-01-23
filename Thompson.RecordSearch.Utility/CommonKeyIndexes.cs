using LegalLead.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility
{

    public static class CommonKeyIndexes
    {

        public static readonly string RowIndex =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.RowIndex);
        public static readonly string ParentElement =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.ParentElement);
        public static readonly string ThContainsText =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.ThContainsText);
        public static readonly string ThElement =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.ThElement);
        public static readonly string InnerText =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.InnerText);
        public static readonly string TrElement =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.TrElement);
        public static readonly string BrElementTag =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.BrElementTag);
        public static readonly string Applicant =
            ResourceTable.GetText(ResourceType.FindDefendant, ResourceKeyIndex.Applicant);
        public static readonly string HlinkUri =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.HlinkUri);
        public static readonly string PersonNodeXpath =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.PersonNodeXpath);
        public static readonly string StartDate =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.StartDate);
        public static readonly string EndDate =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.EndDate);
        public static readonly string CriminalCaseInclusion =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.CriminalCaseInclusion);
        public static readonly string CaseStyle =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.CaseStyle);
        public static readonly string SetText =
            ResourceTable.GetText(ResourceType.ActionName, ResourceKeyIndex.SetText);
        public static readonly string DateTimeShort =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.DateTimeShort);
        public static readonly string SearchComboIndex =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.SearchComboIndex);
        public static readonly string CaseSearchType =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.CaseSearchType);
        public static readonly string DistrictSearchType =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.DistrictSearchType);
        public static readonly string StartDateQuery =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.StartDateQuery);
        public static readonly string EndingDateQuery =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.EndingDateQuery);
        public static readonly string SetComboIndexQuery =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.SetComboIndexQuery);
        public static readonly string CriminalLinkQuery =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.CriminalLinkQuery);
        public static readonly string SearchingForElement =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.SearchingForElement);
        public static readonly string ElementNotFound =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ElementNotFound);
        public static readonly string SetSelectElementIndex =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.SetSelectElementIndex);
        public static readonly string SetSelectOptionIndex =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.SetSelectOptionIndex);
        public static readonly string WaitingForElement =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.WaitingForElement);
        public static readonly string ElementTextNotFound =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ElementTextNotFound);
        public static readonly string ElementMatchTextNotFound =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ElementMatchTextNotFound);
        public static readonly string ElementClassNotFound =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ElementClassNotFound);
        public static readonly string ElementAttributeNotFound =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ElementAttributeNotFound);
        public static readonly string IdLowerCase =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.IdLowerCase);
        public static readonly string GetElementSetIndex =
            ResourceTable.GetText(ResourceType.JscriptCommand, ResourceKeyIndex.GetElementSetIndex);
        public static readonly string ElementFireOnChange =
            ResourceTable.GetText(ResourceType.JscriptCommand, ResourceKeyIndex.ElementFireOnChange);
        public static readonly string ElementGetOptionText =
            ResourceTable.GetText(ResourceType.JscriptCommand, ResourceKeyIndex.ElementGetOptionText);
        public static readonly string ClassAttribute =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.ClassAttribute);
        public static readonly string ClassNameFound =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ClassNameFound);
        public static readonly string NavigateToUrlMessage =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.NavigateToUrlMessage);
        public static readonly string DistrictDash =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.DistrictDash);
        public static readonly string SearchHyperlink =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.SearchHyperlink);

        public static readonly string DistrictHyperlink =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.DistrictHyperlink);

        public static readonly string WaitForNavigation =
            ResourceTable.GetText(ResourceType.ActionName, ResourceKeyIndex.WaitForNavigation);
        public static readonly string WaitForElementExist =
            ResourceTable.GetText(ResourceType.ActionName, ResourceKeyIndex.WaitForElementExist);
        public static readonly string Click =
            ResourceTable.GetText(ResourceType.ActionName, ResourceKeyIndex.Click);
        public static readonly string ClickElement =
            ResourceTable.GetText(ResourceType.ActionName, ResourceKeyIndex.ClickElement);
        public static readonly string SetControlValue =
            ResourceTable.GetText(ResourceType.ActionName, ResourceKeyIndex.SetControlValue);
        public static readonly string GetElement =
            ResourceTable.GetText(ResourceType.ActionName, ResourceKeyIndex.GetElement);
        public static readonly string SetComboIndex =
            ResourceTable.GetText(ResourceType.ActionName, ResourceKeyIndex.SetComboIndex);
        public static readonly string WebNavInstructionMessage =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.WebNavInstructionMessage);
        public static readonly string IdProperCase =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.IdProperCase);
        public static readonly string XPath =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.XPath);
        public static readonly string ControlSetValue =
            ResourceTable.GetText(ResourceType.JscriptCommand, ResourceKeyIndex.ControlSetValue);

        public static readonly string Password =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.Password);
        public static readonly string PasswordMask =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.PasswordMask);
        public static readonly string SettingControlValue =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.SettingControlValue);
        public static readonly string ClickElementJs =
            ResourceTable.GetText(ResourceType.JscriptCommand, ResourceKeyIndex.ClickElementJs);

        public static readonly string ClickingOnElement =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ClickingOnElement);
        public static readonly string DateFiledOnTextBox =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.DateFiledOnTextBox);
        public static readonly string OpenHtmlTag =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.OpenHtmlTag);
        public static readonly string CloseHtmlTag =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.CloseHtmlTag);
        public static readonly string ImageOpenTag =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ImageOpenTag);
        public static readonly string ImageCloseTag =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ImageCloseTag);
        public static readonly string CaseDataXpath =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.CaseDataXpath);
        public static readonly string NavigationControlFile =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.NavigationControlFile);
        public static readonly string CollinCountyCaseType =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.CollinCountyCaseType);
        public static readonly string CaseTypeSelectedIndex =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.CaseTypeSelectedIndex);
        public static readonly string SearchTypeSelectedIndex =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.SearchTypeSelectedIndex);
        public static readonly string SetSelectValue =
            ResourceTable.GetText(ResourceType.ActionName, ResourceKeyIndex.SetSelectValue);
        public static readonly string SearchTypeHyperlink =
            ResourceTable.GetText(ResourceType.ActionName, ResourceKeyIndex.SearchTypeHyperlink);
        public static readonly string CriminalLinkXpath =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.CriminalLinkXpath);
        public static readonly string ProbateLinkXpath =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.ProbateLinkXpath);
        public static readonly string CaseStlyeBoldXpath =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.CaseStlyeBoldXpath);
        public static readonly string IsCriminalSearch =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.IsCriminalSearch);
        public static readonly string BaseUri =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.BaseUri);
        public static readonly string Query =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.Query);
        public static readonly string QueryString =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.QueryString);
        public static readonly string NumberOne =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.NumberOne);
        public static readonly string NumberZero =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.NumberZero);
        public static readonly string NonAddressZipCode =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.NonAddressZipCode);
        public static readonly string NonAddressLine1 =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.NonAddressLine1);
        public static readonly string NonAddressLine2 =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.NonAddressLine2);

        public static readonly string NavigationFileNotFound =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.NavigationFileNotFound);
        public static readonly string SearchSettingFileNotFound =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.SearchSettingFileNotFound);
        public static readonly string SearchRecordCount =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.SearchRecordCount);
        public static readonly string NameProperCase =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.NameProperCase);
        public static readonly string StartingFetchRequest =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.StartingFetchRequest);
        public static readonly string EndingFetchRequest =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.EndingFetchRequest);
        public static readonly string DashedLine =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.DashedLine);
        public static readonly string GeneralLongDate =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.GeneralLongDate);




        public static readonly string StartDateLabel =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.StartDateLabel);
        public static readonly string EndDateLabel =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.EndDateLabel);
        public static readonly string WebsiteLabel =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.WebsiteLabel);
        public static readonly string FileNotFoundError =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.FileNotFoundError);
        public static readonly string DataSourceNotFoundError =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.DataSourceNotFoundError);
        public static readonly string ExcelSourceNotFoundError =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.ExcelSourceNotFoundError);




        public static readonly string SpaceDashSpaceElement =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.SpaceDashSpaceElement);
        public static readonly string CriminalCivilAndFamily =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.CriminalCivilAndFamily);
        public static readonly string JpCountyCourts =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.JpCountyCourts);
        public static readonly string DistrictCourts =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.DistrictCourts);
        public static readonly string SearchColonSpaceElement =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.SearchColonSpaceElement);
        public static readonly string DentonDistrictCaseType =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.DentonDistrictCaseType);



        public static readonly string DentonCountyCaseType =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.DentonCountyCaseType);
        public static readonly string TarrantCountyCaseType =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.TarrantCountyCaseType);
        public static readonly string CollinCountyUserMap =
            ResourceTable.GetText(ResourceType.ParameterName, ResourceKeyIndex.CollinCountyUserMap);
        public static readonly string ElementPipeElement =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ElementPipeElement);
        public static readonly string Error =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.Error);
        public static readonly string FormContextId =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.FormContextId);
        public static readonly string FormStartDate =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.FormStartDate);
        public static readonly string FormEndDate =
            ResourceTable.GetText(ResourceType.Xml, ResourceKeyIndex.FormEndDate);
        public static readonly string EndTime =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.EndTime);
        public static readonly string StartTime =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.StartTime);

        public static readonly string PleaseChooseWebsite =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.PleaseChooseWebsite);
        public static readonly string DataValidationError =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.DataValidationError);
        public static readonly string PleaseCheckStartAndEndDates =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.PleaseCheckStartAndEndDates);
        public static readonly string PleaseCheckStartEndRange =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.PleaseCheckStartEndRange);
        public static readonly string StartDateToEndDateRange =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.StartDateToEndDateRange);
        public static readonly string RangeExceedsMaximunDays =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.RangeExceedsMaximunDays);
        public static readonly string DistrictKeyWord =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.DistrictKeyWord);
        public static readonly string PleaseChooseValidCaseType =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.PleaseChooseValidCaseType);
        public static readonly string PleaseChooseValidCaseSearchType =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.PleaseChooseValidCaseSearchType);
        public static readonly string PleaseChooseValidCourt =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.PleaseChooseValidCourt);
        public static readonly string DateRangeMaxDays =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.DateRangeMaxDays);

        public static readonly string CustomSearchLabel =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.CustomSearchLabel);
        public static readonly string SearchTypeLabel =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.SearchTypeLabel);
        public static readonly string SettingsLabel =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.SettingsLabel);
        public static readonly string PasswordLabel =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.PasswordLabel);

        public static readonly string FormNameCredentials =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.FormNameCredentials);
        public static readonly string FormNameMain =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.FormNameMain);
        public static readonly string FutureKeyWord =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.FutureKeyWord);
        public static readonly string TarrantCountyCustomType =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.TarrantCountyCustomType);
        public static readonly string DefaultKeyWord =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.DefaultKeyWord);
        public static readonly string FormNameDenton =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.FormNameDenton);
        public static readonly string FormNameMdi =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.FormNameMdi);
        public static readonly string JpKeyword =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.JpKeyword);

        public static readonly string ChromeDriver =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ChromeDriver);
        public static readonly string NoDataFoundFromCaseExtract =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.NoDataFoundFromCaseExtract);
        public static readonly string CaseExtractCompleteWouldYouLikeToView =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.CaseExtractCompleteWouldYouLikeToView);


        public static readonly string DataExtractSuccess =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.DataExtractSuccess);
        public static readonly string UnexpectedErrorOccurred =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.UnexpectedErrorOccurred);

        public static readonly string DataNotFound =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.DataNotFound);
        public static readonly string PleaseCheckSourceDataNotFound =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.PleaseCheckSourceDataNotFound);


        public static readonly string ElementSpaceDashSpaceElement =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ElementSpaceDashSpaceElement);
        public static readonly string TextAndAllFilesFilter =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.TextAndAllFilesFilter);
        public static readonly string FormStartup =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.FormStartup);
        public static readonly string ExtensionXml =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ExtensionXml);

        public static readonly string ExtensionXlsm =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ExtensionXlsm);

        public static readonly string ExtensionXlsx =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.ExtensionXlsx);

        /* 
         * 


        DateRangeMaxDays

        
        public static readonly string TextAndAllFilesFilter =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.TextAndAllFilesFilter);
        public static readonly string CaseExtractCompleteWouldYouLikeToView =
            ResourceTable.GetText(ResourceType.FormatString, ResourceKeyIndex.CaseExtractCompleteWouldYouLikeToView);
         
         */
    }
}
