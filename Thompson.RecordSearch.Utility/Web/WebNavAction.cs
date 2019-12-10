using System;
using System.Threading;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    public class WebNavAction : ElementActionBase
    {
        const string actionName = "navigate";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null) throw new System.ArgumentNullException(nameof(item));
            var driver = GetWeb;
            var uri = new Uri(item.Locator.Query);
            driver.Navigate().GoToUrl(uri);
            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}
