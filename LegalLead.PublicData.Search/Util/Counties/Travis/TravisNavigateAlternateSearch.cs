using OpenQA.Selenium;
using System;
using System.Globalization;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class TravisNavigateAlternateSearch : BaseTravisSearchAction
    {
        public override int OrderId => 40;
        public override object Execute()
        {
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            const string controlName = "btnSSSubmit";
            var errmessage = string.Format(CultureInfo.CurrentCulture, "Automation failed to click submit button '{0}'", controlName);
            var locator = By.Id(controlName);
            var button = Driver.FindElement(locator);
            var count = 1;
            var requests = TryClickingElement(executor, button);
            while (count < 10 && requests < 0)
            {
                requests = TryClickingElement(executor, button);
                count++;
            }
            if (requests < 0) throw new ElementNotInteractableException(errmessage);
            WaitForNavigation();

            return true;
        }

        protected static int TryClickingElement(IJavaScriptExecutor jse, IWebElement button)
        {
            const int failed = -1;
            const int success = 1;
            try
            {
                if (jse == null || button == null) return failed;
                jse.ExecuteScript("window.scrollBy(0,400)");
                button.Click();
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return failed;
            }
        }
    }
}