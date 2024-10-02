// ChromeOlderProvider
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Thompson.RecordSearch.Utility.DriverFactory
{
    public class ChromeOlderProvider : BaseChromeProvider, IWebDriverProvider
    {

        public string Name => "Chrome Legacy";

        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <returns></returns>
        public IWebDriver GetWebDriver(bool headless = false)
        {
            var options = GetChromeOptions();
            if (headless)
            {
                options.AddArgument("headless");
            }
            try
            {
                var legacy = $"{GetDriverFileName()}\\Legacy";
                var driver = new ChromeDriver(legacy, options);
                return driver;
            }
            catch (Exception)
            {
                return new ChromeDriver(GetDriverFileName());
                throw;
            }
        }
    }
}
