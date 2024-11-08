using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasSetPager : BaseDallasSearchAction
    {
        public override int OrderId => 50;
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            js = VerifyScript(js);
            WaitForSelector();
            executor.ExecuteScript(js);
            return true;
        }
        private void WaitForSelector()
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(60)) { PollingInterval = TimeSpan.FromMilliseconds(400) };
                wait.Until(w => { return w.TryFindElement(By.XPath(_selector)) != null; });
            }
            catch
            {
                Debug.WriteLine("Failed to find selector");
            }
        }
        protected override string ScriptName { get; } = "select max rows per page";
        private const string _selector = "//span[@class='k-pager-sizes k-label']";
    }
}