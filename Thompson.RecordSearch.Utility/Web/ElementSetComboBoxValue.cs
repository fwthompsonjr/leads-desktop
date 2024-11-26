namespace Thompson.RecordSearch.Utility.Web
{
    using OpenQA.Selenium;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Thompson.RecordSearch.Utility.Dto;
    using Byy = OpenQA.Selenium.By;

    public class ElementSetComboBoxValue : ElementActionBase
    {
        const string actionName = "set-select-value";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            var driver = GetWeb;
            var selector = Byy.CssSelector(item.Locator.Query);
            var elementToClick = driver.FindElement(selector);
            var id = elementToClick.GetAttribute("id");
            var getElement = string.Format("document.getElementById('{0}')", id);
            var jv = new
            {
                setIndex = $"{getElement}.selectedIndex={item.ExpectedValue};",
                change = $"{getElement}.onchange();",
                optionText = $"var sel = {getElement};\nreturn sel.options[sel.selectedIndex].text;"
            };
            if (string.IsNullOrEmpty(item.DisplayName))
            {
                return;
            }
            //document.getElementById('personlist').value=Person_ID;
            var jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript(jv.setIndex);
            jse.ExecuteScript(jv.change);
            var objText = Convert.ToString(jse.ExecuteScript(jv.optionText));
            Debug.WriteLine($"Object {id} option value set to: {objText}");
            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}
