﻿using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Thompson.RecordSearch.Utility.Addressing;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class CollinWebInteractive : TarrantWebInteractive
    {
        public string Credential { get; set; }

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

        public override WebFetchResult Fetch(CancellationToken token)
        {
            // settings have been retrieved from the constructor
            // get any output file to store data from extract
            const StringComparison ccic = StringComparison.OrdinalIgnoreCase;
            var startingDate = GetParameterValue<DateTime>(CommonKeyIndexes.StartDate);
            var endingDate = GetParameterValue<DateTime>(CommonKeyIndexes.EndDate);
            var peopleList = new List<PersonAddress>();
            WebFetchResult webFetch = null;
            while (startingDate.CompareTo(endingDate) <= 0)
            {
                if (token.IsCancellationRequested) break;
                var results = new SettingsManager().GetOutput(this);

                // need to open the navigation file(s)
                var steps = new List<NavigationStep>();
                var navigationFile = GetParameterValue<string>(CommonKeyIndexes.NavigationControlFile);
                var sources = navigationFile.Split(',').ToList();
                var cases = new List<HLinkDataRow>();
                sources.ForEach(s => steps.AddRange(GetAppSteps(s).Steps));
                var caseTypes = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.CollinCountyCaseType);
                var caseTypeId = GetParameterValue<int>(CommonKeyIndexes.CaseTypeSelectedIndex);
                var searchTypeId = GetParameterValue<int>(CommonKeyIndexes.SearchTypeSelectedIndex);
                var selectedCase = caseTypes.DropDowns[caseTypeId];
                if (!string.IsNullOrEmpty(Credential))
                {

                    var loginActions = steps.FindAll(x =>
                        (x.ExpectedValue ?? "").Equals("collinCountyUserMap", ccic));
                    loginActions.ForEach(x => x.ExpectedValue = Credential);
                }
                // set special item values
                var caseTypeSelect = steps.First(x =>
                    x.ActionName.Equals(CommonKeyIndexes.SetSelectValue, ccic));
                caseTypeSelect.ExpectedValue = caseTypeId.ToString(CultureInfo.CurrentCulture.NumberFormat);
                var searchSelect = steps.First(x =>
                    x.DisplayName.Equals(CommonKeyIndexes.SearchTypeHyperlink, ccic));
                searchSelect.Locator.Query = selectedCase.Options[searchTypeId].Query;
                webFetch = SearchWeb(results, steps, startingDate, startingDate,
                    ref cases, out var people);
                peopleList.AddRange(people);
                webFetch.PeopleList = peopleList;
                startingDate = startingDate.AddDays(1);
            }
            return webFetch;
        }

        private WebFetchResult SearchWeb(XmlContentHolder results, List<NavigationStep> steps, DateTime startingDate, DateTime endingDate, ref List<HLinkDataRow> cases, out List<PersonAddress> people)
        {
            IWebDriver driver = WebUtilities.GetWebDriver(DriverReadHeadless);

            try
            {

                var assertion = new ElementAssertion(driver);
                var caseList = string.Empty;
                ElementActions.ForEach(x => x.GetAssertion = assertion);
                ElementActions.ForEach(x => x.GetWeb = driver);
                var formatDate = CultureInfo.CurrentCulture.DateTimeFormat;
                AssignStartAndEndDate(startingDate, endingDate, formatDate, steps);
                foreach (var item in steps)
                {
                    var actionName = item.ActionName;
                    var action = ElementActions
                        .Find(x =>
                            x.ActionName.Equals(item.ActionName,
                            StringComparison.CurrentCultureIgnoreCase));
                    if (action == null)
                    {
                        continue;
                    }
                    if (item.DisplayName == "case-type-search")
                    {
                        WaitSequence(1, driver);
                    }
                    action.Act(item);
                    if (item.DisplayName == "search-type-hyperlink" || item.DisplayName == "case-type-search")
                    {
                        WaitSequence(4, driver);
                    }
                    cases = ExtractCaseData(results, cases, actionName, action);
                    if (string.IsNullOrEmpty(caseList) && !string.IsNullOrEmpty(action.OuterHtml))
                    {
                        caseList = action.OuterHtml;
                    }
                }
                var dtstring = startingDate.ToString("d", CultureInfo.CurrentCulture);
                var subset = cases.FindAll(c => string.IsNullOrEmpty(c.Address));
                var count = subset.Count;
                subset.ForEach(c =>
                    {
                        var idx = subset.IndexOf(c) + 1;
                        var mssg = $"Date: {dtstring}. Reading record {idx} of {count}";
                        this.EchoProgess(0, count, idx, mssg);
                        GetAddressInformation(driver, this, c);
                    }
                    );

                cases.FindAll(c => c.IsCriminal && !string.IsNullOrEmpty(c.CriminalCaseStyle))
                    .ForEach(d => d.CaseStyle = d.CriminalCaseStyle);

                cases.FindAll(c => c.IsJustice && !string.IsNullOrEmpty(c.CriminalCaseStyle))
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
            }
        }

        private static void WaitSequence(int delay, IWebDriver driver)
        {
            for (int i = 0; i < delay; i++)
            {
                Thread.Sleep(450);
                driver.WaitForNavigation();
                Proceed(driver);
            }
        }

        private static void Proceed(IWebDriver driver)
        {
            var by = By.CssSelector("#proceed-button");
            var element = driver.TryFindElement(by);
            if (element == null) { return; }
            var alert = GetAlert(driver);
            alert?.Accept();
            var command = "document.getElementById('proceed-button').click()";
            var jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript(command);
        }
        private static IAlert GetAlert(IWebDriver driver)
        {
            try
            {
                return driver.SwitchTo().Alert();
            }
            catch (Exception)
            {
                return null;
            }
        }
        protected override List<PersonAddress> ExtractPeople(List<HLinkDataRow> cases)
        {
            if (cases == null)
            {
                return null;
            }

            if (!cases.Any())
            {
                return new List<PersonAddress>();
            }

            var list = new List<PersonAddress>();
            foreach (var item in cases)
            {
                var styleInfo = item.IsCriminal | item.IsJustice ? item.CriminalCaseStyle : GetCaseStyle(item);
                if (item.IsProbate)
                {
                    styleInfo = item.CaseStyle;
                }

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
                if (string.IsNullOrEmpty(person.CaseStyle))
                {
                    var mismatched = $"Case Style Data is empty for {person.CaseNumber}";
                    Console.WriteLine(mismatched);
                }
                list.Add(person);
            }
            return list;
        }



        private string CleanUpAddress(string uncleanAddress)
        {
            const string driverLicense = @"DL: ";
            const string secondLicense = @"SID: ";
            const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
            if (string.IsNullOrEmpty(uncleanAddress))
            {
                return uncleanAddress;
            }
            if (uncleanAddress.Contains(driverLicense))
            {
                var dlstart = uncleanAddress.IndexOf(driverLicense, comparison);
                uncleanAddress = uncleanAddress.Substring(0, dlstart).Replace(driverLicense, "");
            }
            if (uncleanAddress.Contains(secondLicense))
            {
                var dlstart = uncleanAddress.IndexOf(secondLicense, comparison);
                uncleanAddress = uncleanAddress.Substring(0, dlstart).Replace(secondLicense, "");
            }
            if (uncleanAddress.IndexOf("DOB: ", comparison) > 0)
            {
                uncleanAddress = string.Empty;
            }
            if (uncleanAddress.IndexOf("Retained", comparison) > 0)
            {
                uncleanAddress = string.Empty;
            }
            if (uncleanAddress.Equals("Pro Se", comparison))
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
        private void GetAddressInformation(IWebDriver driver, TarrantWebInteractive jsonWebInteractive, HLinkDataRow linkData)
        {
            if (jsonWebInteractive == null)
            {
                return;
            }

            var fmt = GetParameterValue<string>(CommonKeyIndexes.HlinkUri);
            var helper = new ElementAssertion(driver);
            helper.Navigate(string.Format(CultureInfo.CurrentCulture, fmt, linkData.WebAddress));
            driver.WaitForNavigation();
            // we have weird situation where the defendant is sometimes PIr11, PIr12
            // heres where we will get the case style for criminal cases

            FindDefendant(driver, ref linkData);

            // can we get the case-style data here

            driver.Navigate().Back();
        }

        private static void FindDefendant(IWebDriver driver, ref HLinkDataRow linkData)
        {

            var criminalLink = TryFindElement(driver, By.XPath(CommonKeyIndexes.CriminalLinkXpath));
            var elementCaseName = TryFindElement(driver, By.XPath(CommonKeyIndexes.CaseStlyeBoldXpath));
            if (criminalLink != null)
            {
                if (elementCaseName != null)
                {
                    linkData.CriminalCaseStyle = elementCaseName.Text;
                    linkData.IsCriminal = true;
                }
            }
            var probateLink = TryFindElement(driver, By.XPath(CommonKeyIndexes.ProbateLinkXpath));
            if (probateLink != null)
            {
                linkData.IsProbate = true;
            }
            if (linkData.IsJustice && elementCaseName != null)
            {
                linkData.CriminalCaseStyle = elementCaseName.Text;
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
                if (finder.CanFind)
                {
                    break;
                }
            }
        }



        private static void AssignStartAndEndDate(DateTime startingDate, DateTime endingDate, DateTimeFormatInfo formatDate, List<NavigationStep> items)
        {
            if (items == null)
            {
                return;
            }

            if (!items.Any())
            {
                return;
            }

            items.ForEach(item =>
            {
                AssignStartAndEndDate(
                    startingDate,
                    endingDate,
                    formatDate,
                    item);
            });
        }
        private static void AssignStartAndEndDate(DateTime startingDate, DateTime endingDate, DateTimeFormatInfo formatDate, NavigationStep item)
        {
            if (!item.ActionName.Equals(CommonKeyIndexes.SetText,
                                    StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }
            if (item.DisplayName.Equals(CommonKeyIndexes.StartDate,
                StringComparison.CurrentCultureIgnoreCase))
            {
                item.ExpectedValue =
                    startingDate.Date.ToString(CommonKeyIndexes.DateTimeShort, formatDate);
            }
            if (item.DisplayName.Equals(CommonKeyIndexes.EndDate,
                StringComparison.CurrentCultureIgnoreCase))
            {
                item.ExpectedValue =
                    endingDate.Date.ToString(CommonKeyIndexes.DateTimeShort, formatDate);
            }
        }

    }
}
