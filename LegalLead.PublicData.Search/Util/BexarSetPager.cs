using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BexarSetPager : BaseBexarSearchAction
    {
        public override int OrderId => 50;
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            if (!WaitForPager()) return false;

            js = VerifyScript(js);
            executor.ExecuteScript(js);
            return true;
        }
        private bool WaitForPager()
        {
            try
            {
                const int mxwait = 3;
                IWebElement found = null;
                // wait for result grid to exist
                var finder = By.Id("hearingResultsGrid");
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(mxwait)) { PollingInterval = TimeSpan.FromMilliseconds(500) };
                wait.Until(d => CheckForElement(d, finder, out found));
                if (found == null) return false;
                // wait for pager to also exist
                finder = By.TagName("ul");
                var wait2 = new WebDriverWait(Driver, TimeSpan.FromSeconds(mxwait)) { PollingInterval = TimeSpan.FromMilliseconds(500) };
                wait2.Until(d => CheckForElement(d, finder, out found));
                return found != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private static bool CheckForElement(IWebDriver d, By finder, out IWebElement element)
        {
            element = d.TryFindElement(finder);
            return element != null;
        }

        protected override string ScriptName { get; } = "select max rows per page";
    }
}