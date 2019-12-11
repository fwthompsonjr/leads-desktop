using LegalLead.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    /// <summary>
    /// Class defintion for WebInteractive class which is used to 
    /// recieve in bound parameter collection and processes the request
    /// </summary>
    public class WebInteractive : BaseWebIneractive
    {
        #region Constructors
        public WebInteractive()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebInteractive"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public WebInteractive(WebNavigationParameter parameters)
        {

            Parameters = parameters;
            StartDate = GetParameterValue<DateTime>(CommonKeyIndexes.StartDate); 
            EndingDate = GetParameterValue<DateTime>(CommonKeyIndexes.EndDate); 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebInteractive"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endingDate">The ending date.</param>
        public WebInteractive(WebNavigationParameter parameters, DateTime startDate, DateTime endingDate)
        {
            Parameters = parameters;
            StartDate = startDate;
            EndingDate = endingDate;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs web scraping activities to fetches data from web source.
        /// </summary>
        /// <returns></returns>
        public override WebFetchResult Fetch()
        {
            var results = new SettingsManager().GetOutput(this);
            var data = WebUtilities.GetCases(this);

            Result = results.FileName;
            foreach (var dta in data)
            {
                AppendExtraCaseInfo(dta);
                var caseStyle = dta[CommonKeyIndexes.CaseStyle]; // "CaseStyle"];
                results.Append(dta);
            }
            // change output of this item.
            // return the person-address collection
            // and the case-list as html table
            //results.Document
            data.FindAll(x => x.IsCriminal & x.IsMapped & !string.IsNullOrEmpty(x.CriminalCaseStyle))
                .ForEach(y => y.CaseStyle = y.CriminalCaseStyle);

            var caseList = ReadFromFile(Result);
            var personAddresses = results.GetPersonAddresses();
            personAddresses = MapCaseStyle(data, personAddresses);
            personAddresses = CleanUp(personAddresses);
            return new WebFetchResult
            {
                Result = Result,
                CaseList = caseList,
                PeopleList = personAddresses
            };
        }

        private List<PersonAddress> CleanUp(List<PersonAddress> personAddresses)
        {
            var found = personAddresses
                .FindAll(f => 
                f.Address1.Equals("Pro Se", StringComparison.CurrentCultureIgnoreCase));
            if (found.Any())
            {
                foreach (var item in found)
                {
                    item.Zip = CommonKeyIndexes.NonAddressZipCode; // "00000";
                    item.Address1 = CommonKeyIndexes.NonAddressLine1;// "No Address Found";
                    item.Address2 = string.Empty;
                    item.Address3 = CommonKeyIndexes.NonAddressLine2; //"Not, Available 00000";
                }
            }
            found = personAddresses
                .FindAll(f => 
                f.Address1.Equals(CommonKeyIndexes.NonAddressLine1, StringComparison.CurrentCultureIgnoreCase) & string.IsNullOrEmpty(f.Zip));

            if (found.Any())
            {
                foreach (var item in found)
                {
                    item.Zip = CommonKeyIndexes.NonAddressZipCode;
                }
            }
            return personAddresses;
        }

        /// <summary>
        /// Maps the case style.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="personAddresses">The person addresses.</param>
        /// <returns></returns>
        private List<PersonAddress> MapCaseStyle(List<HLinkDataRow> data, 
            List<PersonAddress> personAddresses)
        {
            if (personAddresses == null) return personAddresses;
            if (data == null) return personAddresses;
            data = data.FindAll(x => !string.IsNullOrEmpty(x.Case));
            foreach (var person in personAddresses)
            {
                var caseNumber = person.CaseNumber;
                if (string.IsNullOrEmpty(caseNumber)) continue;
                var dataRow = data.Find(x => x.Case.Equals(caseNumber, 
                    StringComparison.CurrentCultureIgnoreCase));
                if (dataRow == null) continue;
                person.CaseStyle = dataRow.CaseStyle;
            }
            
            return personAddresses;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Appends the extra case information to populate 
        /// non-address fields in the PeopleData collection
        /// </summary>
        /// <param name="dta">The dta.</param>
        private void AppendExtraCaseInfo(HLinkDataRow dta)
        {
            var tableHtml = dta.Data;
            if (tableHtml.Contains(CommonKeyIndexes.ImageOpenTag))
            {
                tableHtml = RemoveElement(tableHtml, CommonKeyIndexes.ImageOpenTag);
            }
            var doc = XmlDocProvider.GetDoc(tableHtml);
            var instructions = SearchSettingDto.GetNonCriminalMapping()
                    .NavInstructions
                    .ToList();
            // if doc.DocumentElement.ChildNodes.Count == 4
            if(dta.IsCriminal)
            {
                instructions =
                    SearchSettingDto.GetCriminalMapping()
                    .NavInstructions
                    .ToList();
            }
            var caseStyle = CommonKeyIndexes.CaseStyle.ToLower(CultureInfo.CurrentCulture);
            foreach (var item in instructions)
            {
                if (dta.IsCriminal & item.Value.Equals(caseStyle, StringComparison.CurrentCultureIgnoreCase)) continue;
                var node = TryFindNode(doc, item.Value);
                if (node == null) continue;
                dta[item.Name] = node.InnerText;
                dta.IsMapped = true;
            }
        }

        /// <summary>
        /// Tries the find node.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="xpath">The xpath.</param>
        /// <returns></returns>
        private static XmlNode TryFindNode(XmlDocument doc, string xpath)
        {
            try
            {
                var node = doc.FirstChild.SelectSingleNode(xpath);
                return node;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return null;
            }
        }

        #endregion

    }
}
