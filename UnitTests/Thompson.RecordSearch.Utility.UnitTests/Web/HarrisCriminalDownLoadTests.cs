using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Web;

namespace Thompson.RecordSearch.Utility.Tests
{
    [TestClass]
    public class HarrisCriminalDataTests : TestingBase
    {
        [TestMethod]
        public void Download_HasACorrectTarget()
        {
            var folder = HarrisCriminalData.DownloadFolder;
            folder.ShouldNotBeNullOrEmpty();
            Directory.Exists(folder).ShouldBeTrue();
        }

        [TestMethod]
        [TestCategory("Integration Only")]
        public void Download_CanGetAFile()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Assert.Inconclusive("This method to be executed in debug mode only.");
            }
            var obj = new HarrisCriminalData();
            IWebDriver driver = GetDriver(false);
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
        [TestCategory("Integration Only")]
        public void Download_CanGetAFile_WithoutADriver()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Assert.Inconclusive("This method to be executed in debug mode only.");
            }
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

        [TestMethod]
        [TestCategory("Integration Only")]
        public async Task Download_CanGet_Async()
        {

            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Assert.Inconclusive("This method to be executed in debug mode only.");
            }
            try
            {
                var starting = new List<CriminalStartType> { CriminalStartType.Download };
                await HarrisCriminalStarting.StartAsync(starting);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + Environment.NewLine + exception.StackTrace);
                throw;
            }
        }
    }
}
