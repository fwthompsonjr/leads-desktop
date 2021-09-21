using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using Thompson.RecordSearch.Utility.Web;

namespace Thompson.RecordSearch.Utility.UnitTests.Web
{
    [TestClass]
    public class HarrisCriminalRealTimeTests
    {
        private const string SrcFile = @"D:\Alpha\LegalLead\Thompson.RecordSearch.Utility\_html\sample-harris-criminal-search-result.html";
        private IWebDriver GetDriver;

        [TestInitialize]
        public void Setup()
        {
            if (GetDriver == null)
            {
                var src = SrcFile.Replace(@"\", "/");
                var url = string.Concat("file:", "///", src);
                GetDriver = new FirefoxDriver
                {
                    Url = url
                };
            }
        }
        [TestCleanup]
        public void CleanUp()
        {
            if (GetDriver != null)
            {
                GetDriver.Close();
                GetDriver.Quit();
                GetDriver.Dispose();
                GetDriver = null;
            }
        }

        [TestMethod]
        public void Criminal_CanIterate()
        {
            const int expected = 4331;
            var obj = new HarrisCriminalRealTime();
            try
            {
                var result = obj.IteratePages(GetDriver);
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Count > 0);
                Assert.AreEqual(expected, result.Count);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
    }
}
