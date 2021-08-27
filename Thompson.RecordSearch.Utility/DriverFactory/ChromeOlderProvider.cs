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
        public IWebDriver GetWebDriver()
        {
            var options = GetChromeOptions();
            try
            {
                var legacy = $"{GetDriverFileName()}\\Legacy";
                var driver = new ChromeDriver(legacy, options);
                Console.WriteLine("Chrome executable location:\n {0}", BinaryFileName());
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
