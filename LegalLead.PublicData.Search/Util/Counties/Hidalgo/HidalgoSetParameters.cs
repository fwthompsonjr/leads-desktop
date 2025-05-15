using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using System.Globalization;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class HidalgoSetParameters : BaseHidalgoSearchAction
    {
        public override int OrderId => 20;
        public override object Execute()
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
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

            var courtIndex = 1;
            if (Parameters.CourtType.Equals("Justice", oic)) courtIndex = 2;
            if (Parameters.CourtType.Equals("District", oic)) courtIndex = 3;
            var courtSelector = GetCourtSelector(courtIndex, Parameters.StartDate);
            js = VerifyScript(js);

            var script = js
                .Replace("{0}", courtSelector)
                .Replace("{1}", Parameters.StartDate)
                .Replace("{2}", Parameters.EndingDate);

            executor.ExecuteScript(script);
            WaitForNavigation();
            return true;
        }

        protected static string GetCourtSelector(int courtId, string startDate)
        {
            if (!DateTime.TryParse(startDate, culture, DateTimeStyles.AssumeLocal, out var date))
                date = DateTime.Now;
            var yy = date.ToString("yy", culture);
            return courtId switch
            {
                _ => $"CL-{yy}*" // this selector for civil county, other selectors are not known
            };
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

        private static readonly CultureInfo culture = CultureInfo.CurrentCulture;
    }
}