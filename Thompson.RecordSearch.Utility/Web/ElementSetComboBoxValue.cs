using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    using Byy = OpenQA.Selenium.By;
    using OpenQA.Selenium.Support.UI;
    using Thompson.RecordSearch.Utility.Dto;
    using OpenQA.Selenium;
    using System.Threading;

    public class ElementSetComboBoxValue : ElementActionBase
    {
        const string actionName = "set-select-value";

        public override string ActionName => actionName;

        public override void Act(Step item)
        {
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
