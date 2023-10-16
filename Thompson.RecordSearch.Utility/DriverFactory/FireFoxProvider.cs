using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.IO;
using System.Reflection;

namespace Thompson.RecordSearch.Utility.DriverFactory
{
    public class FireFoxProvider : IWebDriverProvider
    {
        public string Name => "Firefox";
        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <returns></returns>
        public IWebDriver GetWebDriver(bool headless = false)
        {
            var driver = GetDefaultDriver();
            if (driver != null)
            {
                return driver;
            };
            return new FirefoxDriver(GetDriverFileName());
        }

        private static string _driverFileName;

        /// <summary>
        /// Gets the default driver.
        /// </summary>
        /// <returns></returns>
        private static IWebDriver GetDefaultDriver()
        {
            try
            {
                return new FirefoxDriver();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the name of the chrome driver file.
        /// </summary>
        /// <returns></returns>
        private static string GetDriverFileName()
        {
            if (_driverFileName != null)
            {
                return _driverFileName;
            }

            var execName = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            if (!Directory.Exists(execName))
            {
                _driverFileName = string.Empty;
                return string.Empty;
            }
            _driverFileName = execName;
            return execName;
        }
    }
}
