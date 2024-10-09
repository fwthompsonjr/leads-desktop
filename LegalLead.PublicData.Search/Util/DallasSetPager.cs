using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasSetPager : DallasBaseExecutor
    {
        public override int OrderId => 50;
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            
            WaitForGrid();

            js = VerifyScript(js);
            executor.ExecuteScript(js);
            return true;
        }

        private void WaitForGrid()
        {
            try
            {

                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(500)
                };
                wait.Until(d =>
                {
                    return d.TryFindElement(By.Id("CasesGrid")) != null;
                });
            }
            catch (Exception)
            {
                return;
            }
        }

        protected override string ScriptName { get; } = "select max rows per page";
    }
}