using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.IO;
using System.Reflection;
using Thompson.RecordSearch.Utility.Web;

namespace Thompson.RecordSearch.Utility.UnitTests.Web
{
    [TestClass]
    public class HarrisCriminalRealTimeTests
    {
        // Note: Conditional compilation is used in this file
        // Therefore field initialization/assignment issue is being suppressed.
#pragma warning disable S2933 // Fields that are only assigned in the constructor should be "readonly"

        private static string _srcDirectory;
        private static string _srcFile;
        private static string SrcDirectoryName => _srcDirectory ?? (_srcDirectory = SrcDir());
        private static string SrcFile => _srcFile ?? (_srcFile = Path.Combine(SrcDirectoryName, "_html\\sample-harris-criminal-search-result.html"));
        private IWebDriver GetDriver = null;

#pragma warning restore S2933 // Fields that are only assigned in the constructor should be "readonly"

        [TestInitialize]
        public void Setup()
        {
#if DEBUG
            if (GetDriver == null)
            {
                var src = SrcFile.Replace(@"\", "/");
                var url = string.Concat("file:", "///", src);
                GetDriver = new FirefoxDriver
                {
                    Url = url
                };
            }
#else
            Assert.IsFalse(string.IsNullOrEmpty(SrcFile));
            Assert.IsNull(GetDriver);            
#endif
        }
        [TestCleanup]
        public void CleanUp()
        {
#if DEBUG
            if (GetDriver != null)
            {
                GetDriver.Close();
                GetDriver.Quit();
                GetDriver.Dispose();
                GetDriver = null;
            }
#endif
        }

        [TestMethod]
        public void Criminal_CanIterate()
        {
#if DEBUG
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
#else
            Assert.Inconclusive("Test only runs in debug configuration.");            
#endif
        }

        private static string SrcDir()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return Path.GetDirectoryName(assembly.Location);
        }
    }
}
