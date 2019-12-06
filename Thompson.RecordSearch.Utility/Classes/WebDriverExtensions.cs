using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Thompson.RecordSearch.Utility.Classes
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (driver == null) throw new NullReferenceException(nameof(driver));
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
    }
}
