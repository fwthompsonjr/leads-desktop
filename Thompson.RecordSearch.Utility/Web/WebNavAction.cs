using System.Threading;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    public class WebNavAction : ElementActionBase
    {
        const string actionName = "navigate";

        public override string ActionName => actionName;

        public override void Act(Step item)
        {
            var driver = GetWeb;
            driver.Navigate().GoToUrl(item.Locator.Query);
            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}
