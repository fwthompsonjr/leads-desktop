using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.DriverFactory
{
    public class ChromeProvider : IWebDriverProvider
    {
        public string Name => "Chrome";

        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <returns></returns>
        public IWebDriver GetWebDriver()
        {
            var driver = GetDefaultDriver();
            if (driver != null) return driver;
            var options = new ChromeOptions();
            var binaryName = BinaryFileName();
            if (!string.IsNullOrEmpty(binaryName))
            {
                options.BinaryLocation = binaryName;
            }
            return new ChromeDriver(GetDriverFileName(), options);
        }

        private static string _binaryName;
        private static string _driverFileName;

        /// <summary>
        /// Gets the default driver.
        /// </summary>
        /// <returns></returns>
        private static IWebDriver GetDefaultDriver()
        {
            try
            {
                var options = new ChromeOptions();
                var binaryName = BinaryFileName();
                if (!string.IsNullOrEmpty(binaryName))
                {
                    options.BinaryLocation = binaryName;
                }
                return new ChromeDriver(options);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return null;
            }
        }



        private static string BinaryFileName()
        {
            if (_binaryName != null) return _binaryName;
            var settings = ConfigurationManager.AppSettings
                .AllKeys.ToList().FindAll(x => x.StartsWith("chrome.exe.location",
                StringComparison.CurrentCultureIgnoreCase))
                .Select(x => ConfigurationManager.AppSettings[x])
                .ToList().FindAll(x => File.Exists(x));
            if (settings.Any())
            {
                _binaryName = settings.First();
                return _binaryName;
            }

            DirectoryInfo di = new DirectoryInfo(@"c:\");
            var search = new DirectorySearch(di, "*chrome.exe", 2);
            var found = search.FileList;
            if (found.Any())
            {
                _binaryName = found.First();
                return _binaryName;
            }
            search = new DirectorySearch(di, "*chrome.exe");
            found = search.FileList;
            if (found.Any())
            {
                _binaryName = found.First();
                return _binaryName;
            }
            _binaryName = string.Empty;
            return _binaryName;
        }



        /// <summary>
        /// Gets the name of the chrome driver file.
        /// </summary>
        /// <returns></returns>
        private static string GetDriverFileName()
        {
            if (_driverFileName != null) return _driverFileName;
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
