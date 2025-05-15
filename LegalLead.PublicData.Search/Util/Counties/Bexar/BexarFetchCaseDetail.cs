using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Globalization;
using System.Linq;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BexarFetchCaseDetail : BaseBexarSearchAction
    {
        public override int OrderId => 60;
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            /* 
             wait for element
                results grid:  by - id hearingResultsGrid
                results pager: by - css .k-pager-info
             */
            var selections = new[] {
                By.Id("hearingResultsGrid"),
                By.CssSelector(".k-pager-info"),
                By.CssSelector("a[data-url]")
            }.ToList();
            selections.ForEach(selection => { _ = WaitForElement(selection); });
            js = VerifyScript(js);
            var content = executor.ExecuteScript(js);
            return Convert.ToString(content, CultureInfo.CurrentCulture);
        }

        protected override string ScriptName { get; } = "get list of case items";
        private bool WaitForElement(By selection)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30)) { PollingInterval = TimeSpan.FromSeconds(1) };
                wait.Until(w => w.TryFindElement(selection) != null);
                return true;
            }
            catch (Exception e) when (e is NoSuchElementException || e is WebDriverTimeoutException)
            {
                return false;
            }
        }
    }
}