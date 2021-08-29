using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Thompson.RecordSearch.Utility.Classes
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            if (timeoutInSeconds <= 0) return driver.FindElement(by);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(drv => drv.FindElement(by).Displayed);
            return driver.FindElement(by);
        }
        public static void WaitForNavigation(this IWebDriver driver)
        {

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

        }

        public static void ClickAndOrSetText(this IWebDriver driver, IWebElement elementToClick, string objText = "")
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            if (elementToClick == null) throw new ArgumentNullException(nameof(elementToClick));
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].click();", elementToClick);
            if (!string.IsNullOrEmpty(objText))
            {
                executor.ExecuteScript(string.Format("arguments[0].value = '{0}';", objText), elementToClick);
            }
        }


        public static bool IsElementPresent(this IWebDriver driver, By by)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
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
    }
}
