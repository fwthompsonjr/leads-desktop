using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using Thompson.RecordSearch.Utility.Addressing;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class CollinWebInteractive : TarrantWebInteractive
    {
        private class AddressCondition
        {
            public string Name { get; set; }
            public bool IsViolation { get; set; }
        }
        public CollinWebInteractive(WebNavigationParameter parameters) : base(parameters)
        {
        }

        public CollinWebInteractive(WebNavigationParameter parameters, 
            DateTime startDate, DateTime endingDate) : base(parameters, startDate, endingDate)
        {

        }

        public CollinWebInteractive()
        {
        }

        public override WebFetchResult Fetch()
        {
            // settings have been retrieved from the constructor
            // get any output file to store data from extract
            var startingDate = GetParameterValue<DateTime>("startDate");
            var endingDate = GetParameterValue<DateTime>("endDate");
            var peopleList = new List<PersonAddress>();
            WebFetchResult webFetch = null;
            while (startingDate.CompareTo(endingDate) <= 0)
            {

                var results = new SettingsManager().GetOutput(this);

                // need to open the navigation file(s)
                var steps = new List<Step>();
                var navigationFile = GetParameterValue<string>("navigation.control.file");
                var sources = navigationFile.Split(',').ToList();
                var cases = new List<HLinkDataRow>();
                var people = new List<PersonAddress>();
                sources.ForEach(s => steps.AddRange(GetAppSteps(s).Steps));
                var caseTypes = CaseTypeSelectionDto.GetDto("collinCountyCaseType");
                var caseTypeId = GetParameterValue<int>("caseTypeSelectedIndex");
                var searchTypeId = GetParameterValue<int>("searchTypeSelectedIndex");
                var selectedCase = caseTypes.DropDowns[caseTypeId];
                // set special item values
                var caseTypeSelect = steps.First(x => x.ActionName.Equals("set-select-value"));
                caseTypeSelect.ExpectedValue = caseTypeId.ToString();
                var searchSelect = steps.First(x => x.DisplayName.Equals("search-type-hyperlink"));
                searchSelect.Locator.Query = selectedCase.Options[searchTypeId].Query;
                webFetch = SearchWeb(results, steps, startingDate, startingDate,
                    ref cases, out people);
                peopleList.AddRange(people);
                webFetch.PeopleList = peopleList;
                startingDate = startingDate.AddDays(1);
            }
            return webFetch;
        }

        private WebFetchResult SearchWeb(XmlContentHolder results, List<Step> steps, DateTime startingDate, DateTime endingDate, ref List<HLinkDataRow> cases, out List<PersonAddress> people)
        {
            IWebDriver driver = WebUtilities.GetWebDriver();

            try
            {

                var assertion = new ElementAssertion(driver);
                var caseList = string.Empty;
                ElementActions.ForEach(x => x.GetAssertion = assertion);
                ElementActions.ForEach(x => x.GetWeb = driver);

                foreach (var item in steps)
                {
                    // if item action-name = 'set-text'
                    var actionName = item.ActionName;
                    if (item.ActionName.Equals("set-text"))
                    {
                        if (item.DisplayName.Equals("startDate")) item.ExpectedValue = startingDate.Date.ToString("MM/dd/yyyy");
                        if (item.DisplayName.Equals("endDate")) item.ExpectedValue = endingDate.Date.ToString("MM/dd/yyyy");
                    }
                    var action = ElementActions.FirstOrDefault(x => x.ActionName.Equals(item.ActionName));
                    if (action == null) continue;
                    action.Act(item);
                    cases = ExtractCaseData(results, cases, actionName, action);
                    if (string.IsNullOrEmpty(caseList) && !string.IsNullOrEmpty(action.OuterHtml))
                    {
                        caseList = action.OuterHtml;
                    }
                }
                cases.FindAll(c => string.IsNullOrEmpty(c.Address))
                    .ForEach(c => GetAddressInformation(driver, this, c));

                people = ExtractPeople(cases);


                return new WebFetchResult
                {
                    Result = results.FileName,
                    CaseList = caseList,
                    PeopleList = people
                };

            }
            catch (Exception)
            {
                driver.Quit();
                driver.Dispose();
                throw;
            }
            finally
            {
                driver.Quit();
                driver.Dispose();
            }
        }


        protected override List<PersonAddress> ExtractPeople(List<HLinkDataRow> cases)
        {
            if (cases == null) return null;
            if (!cases.Any()) return new List<PersonAddress>();
            var list = new List<PersonAddress>();
            foreach (var item in cases)
            {
                var styleInfo = GetCaseStyle(item, "td[3]/div");
                var person = new PersonAddress
                {
                    Name = item.Defendant,
                    CaseNumber = item.Case,
                    DateFiled = item.DateFiled,
                    Court = item.Court,
                    CaseType = item.CaseType,
                    CaseStyle = styleInfo
                };
                item.Address = CleanUpAddress(item.Address);
                person = ParseAddress(item.Address, person);
                list.Add(person);
            }
            return list;
        }



        private string CleanUpAddress(string uncleanAddress)
        {
            const string driverLicense = @"DL: ";
            const string secondLicense = @"SID: ";
            if (string.IsNullOrEmpty(uncleanAddress)) {
                return uncleanAddress;
            }
            if(uncleanAddress.Contains(driverLicense))
            {
                var dlstart = uncleanAddress.IndexOf(driverLicense);
                uncleanAddress = uncleanAddress.Substring(0, dlstart).Replace(driverLicense, "");
            }
            if (uncleanAddress.Contains(secondLicense))
            {
                var dlstart = uncleanAddress.IndexOf(secondLicense);
                uncleanAddress = uncleanAddress.Substring(0, dlstart).Replace(secondLicense, "");
            }
            if(uncleanAddress.IndexOf("DOB: ") > 0)
            {
                uncleanAddress = string.Empty;
            }
            if (uncleanAddress.IndexOf("Retained") > 0)
            {
                uncleanAddress = string.Empty;
            }
            if(uncleanAddress.Equals("Pro Se"))
            {
                uncleanAddress = string.Empty;
            }
            return uncleanAddress;
        }

        private void GetAddressInformation(IWebDriver driver, TarrantWebInteractive jsonWebInteractive, HLinkDataRow linkData)
        {
            var fmt = GetParameterValue<string>("hlinkUri");
            var xpath = GetParameterValue<string>("personNodeXpath");
            var addrpath = GetParameterValue<string>("personParentXath");
            var childpath = GetParameterValue<string>("childTdXath");
            var helper = new ElementAssertion(driver);
            helper.Navigate(string.Format(fmt, linkData.WebAddress));
            driver.WaitForNavigation();
            // we have weird situation where the defendant is sometimes PIr11, PIr12
           
            FindDefendant(driver, ref linkData);
            
            // can we get the case-style data here

            driver.Navigate().Back();
        }

        private void FindDefendant(IWebDriver driver, ref HLinkDataRow linkData)
        {

            var finders = new List<FindDefendantBase>
            {
                new FindMultipleDefendantMatch(),
                new FindDefendantByWordMatch(),
                new FindPrincipalByWordMatch(),
                new FindPetitionerByWordMatch(),
                new FindApplicantByWordMatch(),
                new FindDefendantByCondemneeMatch(),
                new FindOwnerByWordMatch()
            };

            foreach (var finder in finders)
            {
                finder.Find(driver, linkData);
                if (finder.CanFind) break;
            }
        }



    }
}
