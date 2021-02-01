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
            var options = new ChromeOptions();
            var binaryName = BinaryFileName();
            if (!string.IsNullOrEmpty(binaryName))
            {
                options.BinaryLocation = binaryName;
            }
            try
            {
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

        private static string _binaryName;
        private static string _driverFileName;



        private static string BinaryFileName()
        {
            if (_binaryName != null) return _binaryName;
            _binaryName = WebUtilities.GetChromeBinary();
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
