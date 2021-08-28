using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Shouldly;
using System;
using System.IO;
using Thompson.RecordSearch.Utility.Web;

namespace Thompson.RecordSearch.Utility.Tests
{
    [TestClass]
    public class HarrisCriminalCaseStyleTests : TestingBase
    {
        [TestMethod]
        public void Download_HasACorrectTarget()
        {
            var obj = new HarrisCriminalCaseStyle();
            var folder = obj.DownloadFolder;
            folder.ShouldNotBeNullOrEmpty();
            Directory.Exists(folder).ShouldBeTrue();
        }
        [TestMethod]
        public void CaseStyle_CanGetSingleRecord()
        {
            const string caseNumber = "173333901010";
            var obj = new HarrisCriminalCaseStyle();
            IWebDriver driver = GetDriver();
            try
            {
                var result = obj.GetData(driver, caseNumber);
                result.ShouldNotBeNull();
                result.Count.ShouldBeGreaterThan(0);
            }
            finally
            {
                driver?.Close();
                driver?.Quit();
                KillProcess("chromedriver");
            }
        }
    }
}
