using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    using Byy = OpenQA.Selenium.By;

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
