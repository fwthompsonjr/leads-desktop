using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using StructureMap.Building;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Common
{
    internal static class CommonCaseLinkIterator
    {
        public static List<string> GetCaseNumbers(this ICountySearchAction search,
            string elementWaitLocator = "",
            string getLinkCollectionScript = "")
        {
            var fallback = new List<string>();
            try
            {
                var collector = new FortBendGetLinkCollection()
                {
                    Driver = search.Driver,
                    ExternalExecutor = search.Driver.GetJsExecutor(),
                    Parameters = search.Parameters
                };
                if (string.IsNullOrEmpty(elementWaitLocator)) elementWaitLocator = "//table[@border='0'][@cellpadding='2']";
                var selector = By.XPath(elementWaitLocator);
                if (!ElementWait(selector, collector.Driver)) return fallback;
                var useAlternateScript = !string.IsNullOrEmpty(getLinkCollectionScript);
                var collection = useAlternateScript ?
                    collector.ExternalExecutor.ExecuteScript(getLinkCollectionScript) :
                    collector.Execute();
                if (collection is not string items) return fallback;
                var links = JsonConvert.DeserializeObject<List<string>>(items);
                return links;
            }
            catch (Exception)
            {
                return fallback;
            }

        }



        public static bool ClickCaseNumber(this ICountySearchAction search,
            string caseNo,
            int id,
            string elementWaitLocator = "",
            string clickLinkItemScript = "")
        {
            if (string.IsNullOrEmpty(elementWaitLocator)) elementWaitLocator = "//div[@class='ssCaseDetailCaseNbr']";
            var locator = By.XPath(elementWaitLocator);
            var executor = search.Driver.GetJsExecutor();
            var navigator = new FortBendGetLinkCollectionItem
            {
                Driver = search.Driver,
                ExternalExecutor = executor,
                Parameters = search.Parameters,
                LinkItemId = id
            };

            if (string.IsNullOrEmpty(clickLinkItemScript)) navigator.Execute();
            else executor.ExecuteScript(clickLinkItemScript);
            var handles = search.Driver.WindowHandles.Count;
            if (handles > 1)
                search.Driver.SwitchTo().Window(search.Driver.WindowHandles[^1]);
            if (ElementWait(locator, search.Driver)) return true;
            var findElement = search.Driver
                .TryFindElements(By.TagName("a"))?.FirstOrDefault(a => a.Text.Trim() == caseNo);
            if (findElement == null) return false;
            executor.ExecuteScript("arguments[0].click();", findElement);
            return ElementWait(locator, search.Driver);
        }
        public static List<CaseItemDto> GetCaseItems(
            this ICountySearchAction search,
            List<string> links,
            Func<object, CaseItemDto> mapper,
            string js)
        {
            var blnError = false;
            var alldata = new List<CaseItemDto>();
            var locator = By.XPath("//div[@class='ssCaseDetailCaseNbr']");
            var executor = search.Driver.GetJsExecutor();
            var navigator = new FortBendGetLinkCollectionItem()
            {
                Driver = search.Driver,
                ExternalExecutor = executor,
                Parameters = search.Parameters
            };
            var mx = links.Count;
            var currentDate = search.Parameters.StartDate;
            links.ForEach(link =>
            {
                var indx = links.IndexOf(link);
                var message = $"Date: {currentDate} Reading item: {indx + 1} of {mx}";
                search.Interactive?.EchoProgess(0, mx, indx + 1, message, true);
                if (!blnError)
                {
                    var id = links.IndexOf(link);
                    navigator.LinkItemId = id;
                    navigator.Execute();
                    if (ElementWait(locator, search.Driver))
                    {
                        var person = mapper(executor.ExecuteScript(js));
                        if (person != null) alldata.Add(person);
                    }
                    else
                    {
                        blnError = true;
                        var findElement = search.Driver
                        .TryFindElements(By.TagName("a"))?.FirstOrDefault(a => a.Text.Trim() == link);
                        if (findElement != null)
                        {
                            executor.ExecuteScript("arguments[0].click();", findElement);
                            Thread.Sleep(500);
                            var person = mapper(executor.ExecuteScript(js));
                            if (person != null) alldata.Add(person);
                        }
                    }
                    search.Driver.Navigate().Back();
                }
            });
            search.Interactive?.ReportProgessComplete?.Invoke();
            return alldata;
        }
        private static bool ElementWait(By locator, IWebDriver driver)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5)) { PollingInterval = TimeSpan.FromMilliseconds(300) };
                wait.Until(w =>
                {
                    return w.TryFindElement(locator) != null;
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
