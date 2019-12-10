namespace Thompson.RecordSearch.Utility.Web
{
    using Byy = OpenQA.Selenium.By;
    using Thompson.RecordSearch.Utility.Dto;
    using OpenQA.Selenium;
    using System.Threading;

    public class ElementSetComboBoxValue : ElementActionBase
    {
        const string actionName = "set-select-value";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null) throw new System.ArgumentNullException(nameof(item));
            var driver = GetWeb;
            var selector = Byy.CssSelector(item.Locator.Query);
            var elementToClick = driver.FindElement(selector);
            var id = elementToClick.GetAttribute("id");
            if (string.IsNullOrEmpty(item.DisplayName)) return;
            var objText = item.ExpectedValue;
            var command = string.Format("document.getElementById('{0}').selectedIndex={1};", 
                id, item.ExpectedValue);
            var changecommand = string.Format("document.getElementById('{0}').onchange();",
                id);
            //document.getElementById('personlist').value=Person_ID;
            var jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript(command);
            jse.ExecuteScript(changecommand);

            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}
