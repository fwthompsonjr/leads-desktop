using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Shouldly;
using System.IO;
using Thompson.RecordSearch.Utility.Web;

namespace Thompson.RecordSearch.Utility.Tests
{
    [TestClass]
    public class HarrisCriminalDataTests : TestingBase
    {
        [TestMethod]
        public void Download_HasACorrectTarget()
        {
            var obj = new HarrisCriminalData();
            var folder = obj.DownloadFolder;
            folder.ShouldNotBeNullOrEmpty();
            Directory.Exists(folder).ShouldBeTrue();
        }

        [TestMethod]
        public void Download_CanGetAFile()
        {
            var obj = new HarrisCriminalData();
            IWebDriver driver = GetDriver(true);
            try
            {
                var result = obj.GetData(driver);
                result.ShouldNotBeNull();
                File.Exists(result).ShouldBeTrue();
            }
            finally
            {
                driver?.Close();
                driver?.Quit();
                KillProcess("chromedriver");
            }

        }

        [TestMethod]
        public void Download_CanGetAFile_WithoutADriver()
        {
            var obj = new HarrisCriminalData();
            try
            {
                var result = obj.GetData(null);
                result.ShouldNotBeNull();
                File.Exists(result).ShouldBeTrue();
            }
            finally
            {
                KillProcess("chromedriver");
            }
        }
    }
}
