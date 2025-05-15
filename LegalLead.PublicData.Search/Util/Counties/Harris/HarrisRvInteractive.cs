using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Interfaces;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Util
{
    internal class HarrisRvInteractive : BaseUiInteractive
    {
        public HarrisRvInteractive(WebNavigationParameter parameters) : base(parameters)
        {
            var container = ActionHarrisContainer.GetContainer;
            var collection = container.GetAllInstances<ICountySearchAction>().ToList();
            collection.Sort((a, b) => a.OrderId.CompareTo(b.OrderId));
            ActionItems.AddRange(collection);
            ActionItems.ForEach(a => a.Interactive = this);
        }

        public override WebFetchResult Fetch(CancellationToken token)
        {
            const string countyName = "HARRIS";
            var postsearchtypes = new List<Type> { typeof(NonActionSearch) };
            var driver = GetDriver(DriverReadHeadless);
            var parameters = new DallasSearchProcess();
            var dates = DallasSearchProcess.GetBusinessDays(StartDate, EndingDate);
            var common = ActionItems.FindAll(a => !postsearchtypes.Contains(a.GetType()));
            var postcommon = ActionItems.FindAll(a => postsearchtypes.Contains(a.GetType()));
            var result = new WebFetchResult();
            Iterate(driver, parameters, dates, common, postcommon);
            if (People.Count == 0) return result;
            result.PeopleList = People;
            result.Result = GenerateExcelFile(countyName, 30);
            result.CaseList = JsonConvert.SerializeObject(People);
            return result;
        }

        protected override string GetCourtAddress(string courtType, string court)
        {
            return HarrisCourtLookupService.GetAddress(court);
        }

        protected virtual void Iterate(IWebDriver driver, DallasSearchProcess parameters, List<DateTime> dates, List<ICountySearchAction> common, List<ICountySearchAction> postcommon)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(driver);
                ArgumentNullException.ThrowIfNull(parameters);
                ArgumentNullException.ThrowIfNull(dates);
                ArgumentNullException.ThrowIfNull(common);
                ArgumentNullException.ThrowIfNull(postcommon);
                IterateDateRange(driver, parameters, dates, common);
                IterateItems(driver, parameters, postcommon);

            }
            catch (Exception ex)
            {
                // no action on excpetion thrown.
                Console.WriteLine(ex.Message);
            }
            finally
            {
                driver?.Quit();
            }

        }

        protected virtual void IterateDateRange(IWebDriver driver, DallasSearchProcess parameters, List<DateTime> dates, List<ICountySearchAction> common)
        {
            ArgumentNullException.ThrowIfNull(driver);
            ArgumentNullException.ThrowIfNull(parameters);
            ArgumentNullException.ThrowIfNull(dates);
            ArgumentNullException.ThrowIfNull(common);

            bool isCaptchaNeeded = true;

            dates.ForEach(d =>
            {
                Console.WriteLine($"Date {d:d}. Reading records");
                IsDateRangeComplete = false;
                parameters.SetSearchParameters(d, d, CourtType);
                var find = common.Find(a => a.GetType() == typeof(HarrisFetchPersonDetail));
                if (find is HarrisFetchPersonDetail personFetch) personFetch.ExpectedRecords = 0;
                common.ForEach(a =>
                {
                    if (!IsDateRangeComplete)
                    {
                        isCaptchaNeeded = IterateCommonActions(isCaptchaNeeded, driver, parameters, a, common);
                        var unmapped = Items.FindAll(x => string.IsNullOrEmpty(x.CourtDate));
                        unmapped.ForEach(m => { m.CourtDate = d.ToString("d", CultureInfo.CurrentCulture); });
                    }
                });
            });
            IsDateRangeComplete = false;
            parameters.SetSearchParameters(dates[0], dates[^1], CourtType);
        }

        protected virtual void IterateItems(IWebDriver driver, DallasSearchProcess parameters, List<ICountySearchAction> postcommon)
        {
            ArgumentNullException.ThrowIfNull(driver);
            ArgumentNullException.ThrowIfNull(parameters);
            ArgumentNullException.ThrowIfNull(postcommon);
            var nonames = Items.FindAll(x => string.IsNullOrWhiteSpace(x.PartyName) && !string.IsNullOrWhiteSpace(x.CaseStyle));
            nonames.ForEach(n => n.SetPartyNameFromCaseStyle());
            var casenumbers = Items.Select(s => s.CaseNumber).Distinct().ToList();
            casenumbers.ForEach(i =>
            {
                var p = People.Find(x => x.CaseNumber == i);
                if (p == null)
                {
                    var source = Items.Find(x => x.CaseNumber == i);
                    if (source != null) { AppendPerson(source); }
                }
            });
        }

        private bool IterateCommonActions(bool isCaptchaNeeded, IWebDriver driver, DallasSearchProcess parameters, ICountySearchAction a, List<ICountySearchAction> actions)
        {
            Populate(a, driver, parameters);
            var response = a.Execute();
            if (a is HarrisGetRecordCount _ && response is int expectedCount)
            {
                var find = actions.Find(a => a.GetType() == typeof(HarrisFetchPersonDetail));
                if (find is HarrisFetchPersonDetail personFetch) personFetch.ExpectedRecords = expectedCount;
            }
            if (a is HarrisFetchPersonDetail _ && response is string cases)
            {
                var foundCases = GetData(cases);
                if (foundCases == null || foundCases.Count == 0) return isCaptchaNeeded;
                Items.AddRange(foundCases);
            }
            return isCaptchaNeeded;
        }
    }
}