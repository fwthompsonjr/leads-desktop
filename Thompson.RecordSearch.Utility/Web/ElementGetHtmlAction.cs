using OpenQA.Selenium;
using System;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
// using Thompson.RecordSearch.Utility.Classes.WebElementExtensions;
namespace Thompson.RecordSearch.Utility.Web
{
    public class ElementGetHtmlAction : ElementActionBase
    {
        const string actionName = "get-table-html";

        public override string ActionName => actionName;

        public bool IsProbateSearch { get; private set; }

        public bool IsJusticeSearch { get; private set; }

        public override void Act(NavigationStep item)
        {
            var helper = new CollinWebInteractive();
            var selector = GetSelector(item);
            var element = WaitForElementExisting(GetWeb, selector);
            var outerHtml = element.GetAttribute("outerHTML");
            outerHtml = helper.RemoveElement(outerHtml, "<img");
            // remove colspan? <colgroup>
            outerHtml = helper.RemoveTag(outerHtml, "colgroup");
            // remove the image tags now

            OuterHtml = outerHtml;
            var probateLinkXpath = CommonKeyIndexes.ProbateLinkXpath;
            var justiceLinkXpath = probateLinkXpath.Replace("'Probate'", "'Justice'");
            var probateLink =
                GetWeb.TryFindElement(
                    By.XPath(probateLinkXpath));
            var justiceLocation =
                GetWeb.TryFindElement(
                    By.XPath(justiceLinkXpath));
            var isCollinCounty = GetWeb.Url.Contains("co.collin.tx.us");

            IsProbateSearch = probateLink != null;
            IsJusticeSearch = isCollinCounty && justiceLocation != null;
        }


        private static IWebElement WaitForElementExisting(IWebDriver driver, By by)
        {
            try
            {
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(500)
                };
                wait.Until(d => { return d.TryFindElement(by) != null; });
                return driver.TryFindElement(by);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
