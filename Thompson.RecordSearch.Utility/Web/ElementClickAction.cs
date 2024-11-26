using OpenQA.Selenium;
using System.Diagnostics;
using System.Threading;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    public class ElementClickAction : ElementActionBase
    {
        const string actionName = "click";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            var driver = GetWeb;
            var selector = GetSelector(item);
            var elementToClick = driver.FindElement(selector);
            Debug.WriteLine("Element click action -- : " + selector);
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].click();", elementToClick);
            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }

    }
}
