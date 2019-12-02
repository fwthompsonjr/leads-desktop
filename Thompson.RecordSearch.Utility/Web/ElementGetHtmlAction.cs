using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    public class ElementGetHtmlAction : ElementActionBase
    {
        const string actionName = "get-table-html";

        public override string ActionName => actionName;
        
        public override void Act(Step item)
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
        }
    }
}
