using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Thompson.RecordSearch.Utility.Classes;

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

            var directoryName = ContextManagment.AppDirectory;
            var options = new ChromeOptions();
            // options.AddArgument("--window-size=1440,945");
            options.AddArgument("--start-maximized");
            IWebDriver driver = new ChromeDriver(directoryName, options);
            Assert.IsNotNull(driver);
            Assert.IsInstanceOfType(driver, typeof(IWebDriver));
            return driver;
        }
    }
}
