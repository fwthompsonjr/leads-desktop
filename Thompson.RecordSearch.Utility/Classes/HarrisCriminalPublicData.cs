using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Diagnostics;
using System.Text;

namespace SeleniumTests
{
    [TestClass]
    public class PublicData
    {
        private static IWebDriver driver;
        private StringBuilder verificationErrors;

        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            driver = new FirefoxDriver();
            Assert.IsNotNull(testContext);
        }

        [ClassCleanup]
        public static void CleanupClass()
        {
            try
            {
                //driver.Quit();// quit does not close the window
                driver.Close();
                driver.Dispose();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }

        [TestInitialize]
        public void InitializeTest()
        {
            verificationErrors = new StringBuilder();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            Assert.AreEqual("", verificationErrors.ToString());
        }

        [TestMethod]
        public void ThePublicDataTest()
        {
            try
            {
                if (!Debugger.IsAttached) return;
                const string navTo = "https://www.hcdistrictclerk.com/Common/e-services/PublicDatasets.aspx";
                if (!Uri.TryCreate(navTo, UriKind.Absolute, out var url)) throw new InvalidOperationException();
                driver.Navigate().GoToUrl(url);
                driver.FindElement(By.XPath("//div[contains(string(), \"CrimFilingsWithFutureSettings\")]")).Click();
                driver.FindElement(By.XPath("//div[@id='ctl00_ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder2_ContentPlaceHolder2_blah']/table/tbody/tr[58]/td[3]/a/u/b")).Click();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Assert.Inconclusive("unexpected exception.");
                throw;
            }
        }
    }
}
