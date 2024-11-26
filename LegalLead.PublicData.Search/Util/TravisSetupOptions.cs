
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class TravisSetupOptions : BaseTravisSearchAction
    {
        public override int OrderId => 20;
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            WaitForControls(Driver);
            js = VerifyScript(js);
            executor.ExecuteScript(js);
            return true;
        }
        protected static void WaitForControls(IWebDriver driver)
        {
            const string tilde = "~0";
            const string path = "//input[@name = '~0']";
            var controlNames = new[] { "SearchBy", "CaseStatusType" };
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60))
            {
                PollingInterval = TimeSpan.FromMilliseconds(500)
            };
            try
            {
                wait.Until(d =>
                {
                    var count = 0;
                    for (var i = 0; i < controlNames.Length; i++)
                    {
                        var query = path.Replace(tilde, controlNames[i]);
                        var locator = By.XPath(query);
                        if (d.TryFindElement(locator) != null) count++;
                    }
                    return count == controlNames.Length;
                });
            }
            catch
            {
                // intentionally left blank
            }

        }
        protected override string ScriptName { get; } = "select search by case";
    }
}