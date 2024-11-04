using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Globalization;

namespace Thompson.RecordSearch.Utility.Classes
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            if (timeoutInSeconds <= 0)
            {
                return driver.FindElement(by);
            }

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(drv => drv.FindElement(by).Displayed);
            return driver.FindElement(by);
        }
        public static void WaitForNavigation(this IWebDriver driver)
        {

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30)) { PollingInterval = TimeSpan.FromSeconds(5) };

            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

        }

        public static void ClickAndOrSetText(this IWebDriver driver, IWebElement elementToClick, string objText = "")
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            if (elementToClick == null)
            {
                throw new ArgumentNullException(nameof(elementToClick));
            }

            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].click();", elementToClick);
            if (!string.IsNullOrEmpty(objText))
            {
                var script = string.Format(CultureInfo.InvariantCulture, "arguments[0].value = '{0}';", objText);
                executor.ExecuteScript(script, elementToClick);
            }
        }


        public static void ClearText(this IWebDriver driver, IWebElement elementToClick)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            if (elementToClick == null)
            {
                throw new ArgumentNullException(nameof(elementToClick));
            }

            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].value = '';", elementToClick);
        }
        public static void SetText(this IWebDriver driver, IWebElement elementToClick, string objText)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            if (elementToClick == null)
            {
                throw new ArgumentNullException(nameof(elementToClick));
            }

            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            var script = string.Format(CultureInfo.InvariantCulture, "arguments[0].value = '{0}';", objText);
            executor.ExecuteScript(script, elementToClick);
        }


        public static bool IsElementPresent(this IWebDriver driver, By by)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            try
            {
                var assertion = new ElementAssertion(driver);
                assertion.WaitForElementExist(by, "Element");
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static bool AreElementsPresent(this IWebDriver driver, By by)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            try
            {
                var assertion = new ElementAssertion(driver);
                assertion.WaitForElementsToExist(by, "ElementCollection", 5);
                driver.FindElements(by);
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // intentional to catch all exceptions
                return false;
            }
        }
    }
}
