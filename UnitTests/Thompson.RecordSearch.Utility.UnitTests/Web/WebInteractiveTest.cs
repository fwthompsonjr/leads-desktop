using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Tests
{
    [TestClass]
    public class WebInteractiveTest
    {

        [TestMethod]
        [TestCategory("Web.Configuration.Validation")]
        public void CanInitialize()
        {
            var settings = SettingsManager.GetNavigation();
            var sttg = settings.First();
            var startDate = DateTime.Now.AddMonths(-5);
            var endingDate = DateTime.Now.AddMonths(-4);
            var webactive = new WebInteractive(sttg, startDate, endingDate);
            Assert.IsNotNull(webactive);
            Assert.AreEqual(sttg, webactive.Parameters);
            Assert.AreEqual(startDate, webactive.StartDate);
            Assert.AreEqual(endingDate, webactive.EndingDate);
        }


        [TestMethod]
        [TestCategory("Web.Integration")]
        public void ValidateChromePathTest()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Assert.Inconclusive("This method to be executed in debug mode only.");
            }
            DirectoryInfo di = new DirectoryInfo(@"c:\");
            var search = new DirectorySearch(di, "*chrome.exe", 2);
            var found = search.FileList;
            Assert.IsTrue(found.Any());
            Console.WriteLine(found.First());
        }


        [TestMethod]
        public async Task ValidatePathAsync()
        {
            if (!Debugger.IsAttached) return;
            var fileName = await Task.Run(() => WebUtilities.GetChromeBinary());
            Assert.IsFalse(string.IsNullOrEmpty(fileName));
        }


        [TestMethod]
        [TestCategory("Web.Integration")]
        public void CanFetchDentonCounty()
        {
            if (!CanExecuteFetch())
            {
                return;
            }

            var settings = SettingsManager.GetNavigation();
            var sttg = settings.First();
            var startDate = DateTime.Now.Date.AddDays(-4);
            var endingDate = DateTime.Now.Date.AddDays(-1);
            var keyZero = new WebNavigationKey { Name = "SearchComboIndex", Value = "2" };
            var caseSearch = new WebNavigationKey
            {
                Name = "CaseSearchType",
                Value = "//a[@class='ssSearchHyperlink'][contains(text(),'District Court')]"
            };
            var districtSearch = new WebNavigationKey
            {
                Name = "DistrictSearchType",
                Value = "/html/body/table/tbody/tr[2]/td/table/tbody/tr[1]/td[2]/a[2]"
            };
            if (sttg.Keys == null)
            {
                sttg.Keys = new System.Collections.Generic.List<WebNavigationKey> {
                    keyZero
                };
            }
            // add key for combo-index

            var searchTypeIndex = sttg.Keys.FirstOrDefault(x => x.Name.Equals("SearchComboIndex"));
            if (searchTypeIndex == null)
            {
                sttg.Keys.Add(keyZero);
            }
            // add key for case-search

            var caseTypeIndex = sttg.Keys.FirstOrDefault(x => x.Name.Equals("CaseSearchType"));
            if (caseTypeIndex == null)
            {
                sttg.Keys.Add(caseSearch);
            }
            // add key for district-search

            var districtTypeIndex = sttg.Keys.FirstOrDefault(x => x.Name.Equals("DistrictSearchType"));
            if (districtTypeIndex == null)
            {
                sttg.Keys.Add(districtSearch);
            }
            var webactive = new WebInteractive(sttg, startDate, endingDate);
            var found = webactive.Fetch();
            Assert.IsNotNull(found);

            WriteToExcel(found);
        }


        [TestMethod]
        [TestCategory("Web.Integration")]
        public void CanFetchDentonCountyNormal()
        {
            if (!CanExecuteFetch())
            {
                return;
            }

            var settings = SettingsManager.GetNavigation();
            var sttg = settings.First();
            var startDate = DateTime.Now.Date.AddDays(-4);
            var endingDate = DateTime.Now.Date.AddDays(-1);
            var webactive = new WebInteractive(sttg, startDate, endingDate);
            var found = webactive.Fetch();
            Assert.IsNotNull(found);

            WriteToExcel(found);
        }
        private void WriteToExcel(WebFetchResult found)
        {
            ExcelWriter.WriteToExcel(found);
        }

        private bool CanExecuteFetch()
        {
            return ExecutionManagement.CanExecuteFetch();

        }
    }
}
