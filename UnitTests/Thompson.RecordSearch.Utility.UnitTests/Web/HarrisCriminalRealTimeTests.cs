using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var obj = new HarrisCriminalRealTime();
            try
            {
                obj.IteratePages(GetDriver);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }
    }
}
