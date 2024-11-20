using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Common
{
    internal static class CommonCaseLinkIterator
    {
        public static List<string> GetCaseNumbers(this ICountySearchAction search)
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
                var selector = By.XPath("//table[@border='0'][@cellpadding='2']");
                if (!ElementWait(selector, collector.Driver)) return fallback;
                var collection = collector.Execute();
                if (collection is not string items) return fallback;
                var links = JsonConvert.DeserializeObject<List<string>>(items);
                return links;
            }
            catch (Exception)
            {
                return fallback;
            }

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
            links.ForEach(link =>
            {
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
