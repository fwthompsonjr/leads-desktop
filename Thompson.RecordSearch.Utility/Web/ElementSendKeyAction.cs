using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{

    using Byy = OpenQA.Selenium.By;

    public class ElementSendKeyAction : ElementActionBase
    {
        const string actionName = "send-key";

        public override string ActionName => actionName;

        public override void Act(Step item)
        {
            if (item == null) throw new System.ArgumentNullException(nameof(item));
            var driver = GetWeb;
            var selector = Byy.CssSelector(item.Locator.Query);
            var elementToClick = driver.FindElement(selector);
            if (string.IsNullOrEmpty(item.DisplayName)) return;
            var objText = item.ExpectedValue;
            elementToClick.Click();
            var jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript("arguments[0].blur();", elementToClick);
        }
    }
}
