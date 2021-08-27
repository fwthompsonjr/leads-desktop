using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
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


        protected IWebDriver GetDriver()
        {
            var provider = new ChromeOlderProvider();
            IWebDriver driver = provider.GetWebDriver();
            Assert.IsNotNull(driver);
            Assert.IsInstanceOfType(driver, typeof(IWebDriver));
            return driver;
        }

    }
}
