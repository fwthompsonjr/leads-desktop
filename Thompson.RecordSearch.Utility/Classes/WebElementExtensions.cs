using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;

namespace Thompson.RecordSearch.Utility.Classes
{
    public static class WebElementExtensions
    {

        //
        // Summary:
        //     Finds the first OpenQA.Selenium.IWebElement using the given method.
        //
        // Parameters:
        //   by:
        //     The locating mechanism to use.
        //
        // Returns:
        //     The first matching OpenQA.Selenium.IWebElement on the current context.
        //
        // Exceptions:
        //   T:OpenQA.Selenium.NoSuchElementException:
        //     If no element matches the criteria.
        public static IWebElement TryFindElement(this IWebElement webElement, By by )
        {
            try
            {
                return webElement.FindElement(by);
            }
            catch (Exception)
            {
                return null;
            }
        }
        //
        // Summary:
        //     Finds the first OpenQA.Selenium.IWebElement using the given method.
        //
        // Parameters:
        //   by:
        //     The locating mechanism to use.
        //
        // Returns:
        //     The first matching OpenQA.Selenium.IWebElement on the current context.
        //
        // Exceptions:
        //   T:OpenQA.Selenium.NoSuchElementException:
        //     If no element matches the criteria.
        public static IWebElement TryFindElement(this IWebDriver driver, By by)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch (Exception)
            {
                return null;
            }
        }


        //
        // Summary:
        //     Finds all OpenQA.Selenium.IWebElement within the current context using the given
        //     mechanism.
        //
        // Parameters:
        //   by:
        //     The locating mechanism to use.
        //
        // Returns:
        //     A System.Collections.ObjectModel.ReadOnlyCollection`1 of all OpenQA.Selenium.IWebElement
        //     matching the current criteria, or an empty list if nothing matches.
        public static ReadOnlyCollection<IWebElement> TryFindElements(this IWebDriver driver, By by)
        {
            try
            {
                return driver.FindElements(by);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
