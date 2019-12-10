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
            var probateLink =
                GetWeb.TryFindElement(
                    By.XPath("//a[@class = 'ssBlackNavBarHyperlink'][contains(text(),'Probate')]"));
            
            IsProbateSearch = probateLink != null;
        }
    }
}
