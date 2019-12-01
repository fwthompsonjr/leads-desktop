using OpenQA.Selenium;
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

    public class ElementClickAction : ElementActionBase
    {
        const string actionName = "click";

        public override string ActionName => actionName;

        public override void Act(Step item)
        {
            var driver = GetWeb;
            var selector = GetSelector(item);
            var elementToClick = driver.FindElement(selector);
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].click();", elementToClick);
            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }

    }
}
