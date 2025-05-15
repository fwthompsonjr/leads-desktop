using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    internal class TarrantRvUiInteractive : BaseUiInteractive
    {
        public TarrantRvUiInteractive(WebNavigationParameter parameters) : base(parameters)
        {
            var container = ActionTarrantContainer.GetContainer;
            var collection = container.GetAllInstances<ICountySearchAction>().ToList();
            collection.Sort((a, b) => a.OrderId.CompareTo(b.OrderId));
            ActionItems.AddRange(collection);
            ActionItems.ForEach(a => a.Interactive = this);
            // bind prompt behavior
            ActionItems.ForEach(a =>
            {
                if (a is BaseTarrantAction tarrant) tarrant.PromptUser = UserPrompt;
            });
        }

        public override WebFetchResult Fetch(CancellationToken token)
        {
            const string countyName = "TARRANT";
            var postsearchtypes = new List<Type> { typeof(NonActionSearch) };
            var driver = GetDriver(DriverReadHeadless);
            var parameters = new DallasSearchProcess();
            var dates = DallasSearchProcess.GetBusinessDays(StartDate, EndingDate);
            var common = ActionItems.FindAll(a => !postsearchtypes.Contains(a.GetType()));
            var postcommon = ActionItems.FindAll(a => postsearchtypes.Contains(a.GetType()));
            parameters.UserSelectedCourtType = UserSelectedCourtType;
            parameters.UserSelectedSearchName = UserSelectedSearchName;
            var result = new WebFetchResult();
            Iterate(driver, parameters, dates, common, postcommon);
            if (People.Count == 0) return result;
            result.PeopleList = People;
            result.Result = GenerateExcelFile(countyName, 10);
            result.CaseList = JsonConvert.SerializeObject(People);
            return result;
        }

        protected override string GetCourtAddress(string courtType, string court)
        {
            return TarrantCourtLookupService.GetAddress(court);
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
                var originalType = parameters.UserSelectedCourtType;
                var collection = GetCourtLocationItems(parameters.UserSelectedCourtType);
                collection.ForEach(c =>
                {
                    parameters.UserSelectedCourtType = c;
                    Console.WriteLine($"Searching {parameters.UserSelectedCourtType} cases");
                    IterateDateRange(driver, parameters, dates, common);
                });
                parameters.UserSelectedCourtType = originalType;
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
                common.ForEach(a =>
                {
                    if (!IsDateRangeComplete)
                    {
                        isCaptchaNeeded = IterateCommonActions(isCaptchaNeeded, driver, parameters, a);
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

        private bool IterateCommonActions(bool isCaptchaNeeded, IWebDriver driver, DallasSearchProcess parameters, ICountySearchAction a)
        {
            Populate(a, driver, parameters);
            if (a is TarrantFetchPersonDetail addressing)
            {
                addressing.Items.Clear();
                var notProcessed = Items.FindAll(i =>
                {
                    if (i is CaseItemDtoTraker t) return !t.IsProcessed;
                    return true;
                });
                addressing.Items.AddRange(notProcessed);
            }
            var response = a.Execute();
            if (a is TarrantFetchCaseList _ && response is string cases)
            {
                var foundCases = GetData(cases);
                if (foundCases == null || foundCases.Count == 0) return isCaptchaNeeded;
                var trackingJs = foundCases.ToJsonString();
                var trackingList = trackingJs.ToInstance<List<CaseItemDtoTraker>>();
                Items.AddRange(trackingList);
            }
            return isCaptchaNeeded;
        }

        [ExcludeFromCodeCoverage]
        private bool UserPrompt()
        {
            return DisplayUserCaptchaHelper.UserPrompt();
        }

        private static List<string> GetCourtLocationItems(string requested)
        {
            var items = new List<string> { requested };
            if (!requested.Equals("All JP Courts")) return items;
            var indexes = new List<int>{1, 2, 3, 4, 5, 6, 7, 8}
            .Select(x => $"JP No. {x}");
            return [.. indexes];

        }
    }
}
