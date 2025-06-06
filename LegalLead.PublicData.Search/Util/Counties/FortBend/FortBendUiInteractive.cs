﻿using LegalLead.PublicData.Search.Classes;
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
    internal class FortBendUiInteractive : BaseUiInteractive
    {
        public FortBendUiInteractive(WebNavigationParameter parameters) : base(parameters)
        {
            var container = ActionFortBendContainer.GetContainer;
            var collection = container.GetAllInstances<ICountySearchAction>().ToList();
            collection.Sort((a, b) => a.OrderId.CompareTo(b.OrderId));
            ActionItems.AddRange(collection);
            ActionItems.ForEach(a => { a.Interactive = this; });
        }

        public override WebFetchResult Fetch(CancellationToken token)
        {
            const string countyName = "FortBend";

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
            result.Result = GenerateExcelFile(countyName.ToUpper(), 110);
            result.CaseList = JsonConvert.SerializeObject(People);
            return result;
        }

        protected override string GetCourtAddress(string courtType, string court)
        {
            return FortBendCourtLookupService.GetAddress(courtType, court);
        }

        protected virtual void Iterate(IWebDriver driver, DallasSearchProcess parameters, List<DateTime> dates, List<ICountySearchAction> common, List<ICountySearchAction> postcommon)
        {
            try
            {
                if (driver == null) throw new ArgumentNullException(nameof(driver));
                if (parameters == null) throw new ArgumentNullException(nameof(parameters));
                if (dates == null) throw new ArgumentNullException(nameof(dates));
                if (common == null) throw new ArgumentNullException(nameof(common));
                if (postcommon == null) throw new ArgumentNullException(nameof(postcommon));

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
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (dates == null) throw new ArgumentNullException(nameof(dates));
            if (common == null) throw new ArgumentNullException(nameof(common));
            bool isCaptchaNeeded = true;
            dates.ForEach(d =>
            {
                IsDateRangeComplete = false;
                parameters.SetSearchParameters(d, d, CourtType);
                common.ForEach(a =>
                {
                    if (!IsDateRangeComplete)
                    {
                        isCaptchaNeeded = IterateCommonActions(isCaptchaNeeded, driver, parameters, common, a);
                        var unmapped = Items.FindAll(x => string.IsNullOrEmpty(x.CourtDate));
                        unmapped.ForEach(m => { m.CourtDate = d.ToString("d", CultureInfo.CurrentCulture); });
                    }
                });
                IsDateRangeComplete = false;
            });
            parameters.SetSearchParameters(dates[0], dates[^1], CourtType);
        }

        protected virtual void IterateItems(IWebDriver driver, DallasSearchProcess parameters, List<ICountySearchAction> postcommon)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (postcommon == null) throw new ArgumentNullException(nameof(postcommon));
            CaseStyles.ForEach(c =>
            {
                var targets = Items.FindAll(x => x.CaseNumber == c.CaseNumber);
                targets.ForEach(y =>
                {
                    y.Address = c.Address;
                    y.PartyName = c.PartyName;
                });
            });
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

        private bool IterateCommonActions(bool isCaptchaNeeded, IWebDriver driver, DallasSearchProcess parameters, List<ICountySearchAction> common, ICountySearchAction a)
        {
            var count = common.Count - 1;
            Populate(a, driver, parameters);
            var response = a.Execute();
            if (a is FortBendFetchCaseList _ && response is string cases) Items.AddRange(GetData(cases));
            if (a is FortBendFetchClickStyle _ && response is string persons) CaseStyles.AddRange(GetData(persons));
            if (a is HidalgoNoCountVerification _ && response is bool hasNoCount && hasNoCount)
            {
                IsDateRangeComplete = true;
            }
            if (common.IndexOf(a) != count) Thread.Sleep(750); // add pause to be more human in interaction
            return isCaptchaNeeded;
        }
    }
}