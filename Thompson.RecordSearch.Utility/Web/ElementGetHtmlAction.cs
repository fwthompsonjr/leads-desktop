using OpenQA.Selenium;
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
            var element = GetWeb.FindElement(selector);
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
    }
}
