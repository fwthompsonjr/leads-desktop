using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace LegalLead.PublicData.Search.Common
{
    internal static class NavigationExtensions
    {
        public static void WaitForDocumentReady(
            this IWebDriver driver,
            IJavaScriptExecutor jsexec,
            TimeSpan tswait,
            TimeSpan tsPoolingInterval)
        {
            const string request = "return document.readyState";
            const string response = "complete";
            var wait = new WebDriverWait(driver, tswait) { PollingInterval = tsPoolingInterval };
            wait.Until(driver1 =>
            {
                var obj = jsexec.ExecuteScript(request);
                if (obj is not string pagestatus) return false;
                return pagestatus.Equals(response);
            });
        }

        public static IJavaScriptExecutor GetJsExecutor(
            this IWebDriver driver)
        {
            if (driver is IJavaScriptExecutor exec) return exec;
            return null;
        }
    }
}
