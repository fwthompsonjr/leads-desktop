using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class FortBendSetParameters : BaseFortBendSearchAction
    {
        public override int OrderId => 20;
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            if (string.IsNullOrEmpty(Parameters.StartDate))
                throw new NullReferenceException(Rx.ERR_START_DATE_MISSING);

            if (string.IsNullOrEmpty(Parameters.EndingDate))
                throw new NullReferenceException(Rx.ERR_END_DATE_MISSING);

            if (string.IsNullOrEmpty(Parameters.CourtType))
                throw new NullReferenceException(Rx.ERR_COURT_TYPE_MISSING);

            // wait for elements
            var locator = By.Id("SearchBy");
            WaitForComboBox(locator);

            js = VerifyScript(js);

            var script = js
                .Replace("{0}", string.Empty)
                .Replace("{1}", Parameters.StartDate)
                .Replace("{2}", Parameters.EndingDate);

            executor.ExecuteScript(script);
            WaitForNavigation();
            return true;
        }
        private void WaitForComboBox(By locator)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10)) { PollingInterval = TimeSpan.FromMilliseconds(500) };
                wait.Until(w =>
                {
                    var item = w.TryFindElement(locator);
                    return item != null;
                });
            }
            catch (Exception)
            {
                Debug.WriteLine("Wait for table object experienced error.");
            }
        }

        protected override string ScriptName { get; } = "populate search parameters";
    }
}