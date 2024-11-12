using OpenQA.Selenium;
using System;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class ElementGetElement : ElementNavigationBase
    {
        public override IWebElement Execute(WebNavInstruction item)
        {
            if (Assertion == null)
            {
                return null;
            }

            if (PageDriver == null)
            {
                return null;
            }
            var selector = GetSelector(item);
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return WaitForElementExisting(PageDriver, selector);
        }

        private static By GetSelector(WebNavInstruction item)
        {

            if (item == null) return null;
            if (item.By == CommonKeyIndexes.IdProperCase)
            {
                return By.Id(item.Value);
            }
            if (item.By == CommonKeyIndexes.XPath)
            {
                return By.XPath(item.Value);
            }
            return null;
        }

        private static IWebElement WaitForElementExisting(IWebDriver driver, By by)
        {
            try
            {
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30)) { 
                    PollingInterval = TimeSpan.FromMilliseconds(500)
                };
                wait.Until(d => { return d.TryFindElement(by) != null; });
                return driver.TryFindElement(by);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
