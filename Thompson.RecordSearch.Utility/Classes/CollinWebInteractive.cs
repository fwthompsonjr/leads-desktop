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
                var caseTypeSelect = steps.First(x => x.ActionName.Equals("set-select-value", StringComparison.CurrentCultureIgnoreCase));
                caseTypeSelect.ExpectedValue = caseTypeId.ToString();
                var searchSelect = steps.First(x => x.DisplayName.Equals("search-type-hyperlink", StringComparison.CurrentCultureIgnoreCase));
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
                    if (item.ActionName.Equals("set-text", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (item.DisplayName.Equals("startDate", StringComparison.CurrentCultureIgnoreCase)) item.ExpectedValue = startingDate.Date.ToString("MM/dd/yyyy");
                        if (item.DisplayName.Equals("endDate", StringComparison.CurrentCultureIgnoreCase)) item.ExpectedValue = endingDate.Date.ToString("MM/dd/yyyy");
                    }
                    var action = ElementActions.FirstOrDefault(x => x.ActionName.Equals(item.ActionName, StringComparison.CurrentCultureIgnoreCase));
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

                cases.FindAll(c => c.IsCriminal && !string.IsNullOrEmpty(c.CriminalCaseStyle))
                    .ForEach(d => d.CaseStyle = d.CriminalCaseStyle);

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
                var styleInfo = item.IsCriminal ? item.CriminalCaseStyle : GetCaseStyle(item, "td[3]/div");
                if (item.IsProbate) styleInfo = item.CaseStyle;
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
                var dlstart = uncleanAddress.IndexOf(driverLicense, StringComparison.CurrentCultureIgnoreCase);
                uncleanAddress = uncleanAddress.Substring(0, dlstart).Replace(driverLicense, "");
            }
            if (uncleanAddress.Contains(secondLicense))
            {
                var dlstart = uncleanAddress.IndexOf(secondLicense, StringComparison.CurrentCultureIgnoreCase);
                uncleanAddress = uncleanAddress.Substring(0, dlstart).Replace(secondLicense, "");
            }
            if(uncleanAddress.IndexOf("DOB: ", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                uncleanAddress = string.Empty;
            }
            if (uncleanAddress.IndexOf("Retained", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                uncleanAddress = string.Empty;
            }
            if(uncleanAddress.Equals("Pro Se", StringComparison.CurrentCultureIgnoreCase))
            {
                uncleanAddress = string.Empty;
            }
            return uncleanAddress;
        }


        /// <summary>
        /// Gets the address information.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="jsonWebInteractive">The json web interactive.</param>
        /// <param name="linkData">The link data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void GetAddressInformation(IWebDriver driver, TarrantWebInteractive jsonWebInteractive, HLinkDataRow linkData)
        {
            if (jsonWebInteractive == null) return;
            var fmt = GetParameterValue<string>("hlinkUri");
            GetParameterValue<string>("personNodeXpath");
            GetParameterValue<string>("personParentXath");
            GetParameterValue<string>("childTdXath");
            var helper = new ElementAssertion(driver);
            helper.Navigate(string.Format(fmt, linkData.WebAddress));
            driver.WaitForNavigation();
            // we have weird situation where the defendant is sometimes PIr11, PIr12
            // heres where we will get the case style for criminal cases

            FindDefendant(driver, ref linkData);
            
            // can we get the case-style data here

            driver.Navigate().Back();
        }

        private static void FindDefendant(IWebDriver driver, ref HLinkDataRow linkData)
        {

            var criminalLink = TryFindElement(driver, By.XPath("//a[@class = 'ssBlackNavBarHyperlink'][contains(text(),'Criminal')]"));
            if (criminalLink != null)
            {
                var elementCaseName = TryFindElement(driver, By.XPath("/html/body/table[3]/tbody/tr/td[1]/b"));
                if (elementCaseName != null)
                {
                    linkData.CriminalCaseStyle = elementCaseName.Text;
                    linkData.IsCriminal = true;
                }
            }
            var probateLink = TryFindElement(driver, By.XPath("//a[@class = 'ssBlackNavBarHyperlink'][contains(text(),'Probate')]"));
            if(probateLink != null)
            {
                linkData.IsProbate = true;
            }
            var finders = new List<FindDefendantBase>
            {
                new FindMultipleDefendantMatch(),
                new FindDefendantByWordMatch(),
                new FindPrincipalByWordMatch(),
                new FindPetitionerByWordMatch(),
                new FindApplicantByWordMatch(),
                new FindDefendantByCondemneeMatch(),
                new FindDefendantByGuardianMatch(),
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
