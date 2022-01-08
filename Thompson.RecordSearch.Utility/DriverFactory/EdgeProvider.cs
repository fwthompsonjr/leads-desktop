// EdgeProvider
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;
using System;
using System.IO;
using System.Reflection;

namespace Thompson.RecordSearch.Utility.DriverFactory
{
    public class EdgeProvider : IWebDriverProvider
    {
        public string Name => "Edge";
        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <returns></returns>
        public IWebDriver GetWebDriver(bool headless = false)
        {
            return new EdgeDriver(GetDriverFileName());
        }

        private static string _driverFileName;


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
