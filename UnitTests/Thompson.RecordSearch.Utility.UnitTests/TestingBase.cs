using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Diagnostics;
using Thompson.RecordSearch.Utility.DriverFactory;

namespace Thompson.RecordSearch.Utility.Tests
{
    public class TestingBase
    {

        private IWebDriver _currentWebDriver;
        protected IWebDriver CurrentWebDriver
        {
            set { _currentWebDriver = value; }
            get { return _currentWebDriver ?? (_currentWebDriver = GetDriver()); }
        }


        protected static IWebDriver GetDriver(bool headless = false)
        {
            var provider = new ChromeOlderProvider();
            IWebDriver driver = provider.GetWebDriver(headless);
            Assert.IsNotNull(driver);
            Assert.IsInstanceOfType(driver, typeof(IWebDriver));
            return driver;
        }


        protected static void KillProcess(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }
    }
}
