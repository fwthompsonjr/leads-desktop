using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thompson.RecordSearch.Utility.Classes;
using System.Linq;
using System.Configuration;
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
            var settings = new SettingsManager().GetNavigation();
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
        public void CanFetchDentonCounty()
        {
            if (!CanExecuteFetch()) return;
            
            var settings = new SettingsManager().GetNavigation();
            var sttg = settings.First();
            var startDate = DateTime.Now.Date.AddDays(-4);
            var endingDate = DateTime.Now.Date.AddDays(-1);
            var keyZero = new WebNavigationKey { Name = "SearchComboIndex", Value = "2" };
            var caseSearch = new WebNavigationKey
            {
                Name = "CaseSearchType",
                Value = "/html/body/table/tbody/tr[2]/td/table/tbody/tr[1]/td[2]/a[1]"
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
            if(searchTypeIndex == null)
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
            if (!CanExecuteFetch()) return;

            var settings = new SettingsManager().GetNavigation();
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
            var settingCanExecute = ConfigurationManager.AppSettings["allow.web.integration"];
            if (settingCanExecute == null) return true;
            var canExec = false;
            if (!bool.TryParse(settingCanExecute, out canExec)) return true;
            return canExec;

        }
    }
}
