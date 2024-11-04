using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BexarSetPager : BaseBexarSearchAction
    {
        public override int OrderId => 50;
        public bool UseElementWait { get; set; } = true;
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            if (UseElementWait && !WaitForPager()) return false;

            js = VerifyScript(js);
            executor.ExecuteScript(js);
            return true;
        }
        private bool WaitForPager()
        {
            try
            {
                const int mxwait = 3;
                bool elementMissing = false;
                IWebElement found = null;
                var selections = elementSelections.ToList();
                selections.ForEach(selection =>
                {
                    if (!elementMissing)
                    {
                        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(mxwait))
                        {
                            PollingInterval = TimeSpan.FromMilliseconds(500)
                        };
                        wait.Until(d => CheckForElement(d, selection, out found));
                        elementMissing = found == null;
                    }
                });
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
        private static readonly By[] elementSelections = new[] {
                    By.Id("hearingResultsGrid"),
                    By.CssSelector(".k-pager-info"),
                    By.XPath("//span[@class='k-pager-info k-label']")
                };
    }
}