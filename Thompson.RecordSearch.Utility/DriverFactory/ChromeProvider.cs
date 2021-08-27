using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Thompson.RecordSearch.Utility.DriverFactory
{
    public class ChromeProvider : BaseChromeProvider, IWebDriverProvider
    {
        public string Name => "Chrome";

        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <returns></returns>
        public IWebDriver GetWebDriver()
        {
            var options = new ChromeOptions();
            var binaryName = BinaryFileName();
            if (!string.IsNullOrEmpty(binaryName))
            {
                options.BinaryLocation = binaryName;
            }
            try
            {
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("download.directory_upgrade", true);
                options.AddUserProfilePreference("download.default_directory", GetDownloadPath());
                var driver = new ChromeDriver(GetDriverFileName(), options);
                Console.WriteLine("Chrome executable location:\n {0}", binaryName);
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
