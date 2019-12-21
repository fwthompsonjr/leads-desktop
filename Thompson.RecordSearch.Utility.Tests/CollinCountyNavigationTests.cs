using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Tests
{
    [TestClass]
    public class CollinCountyNavigationTests
    {
        [TestMethod]
        public void CanGetCaseTypes()
        {
            var caseTypes = CaseTypeSelectionDto.GetDto("collinCountyCaseType");
            Assert.IsNotNull(caseTypes);
        }

        [TestMethod]
        public void CanGetProbateSelection()
        {
            var caseTypes = CaseTypeSelectionDto.GetDto("collinCountyCaseType");
            Assert.IsNotNull(caseTypes);
            Assert.IsTrue(caseTypes.DropDowns.Any(x => x.Id == 1));
            var dropDown = caseTypes.DropDowns.First(x => x.Id == 1);
            Assert.AreEqual(dropDown.Name, "probate courts");

        }

        [TestMethod]
        [TestCategory("collin.county.actions")]
        [TestCategory("Web.Integration")]
        public void CanGetFromCollinInteractive()
        {
            if (!ExecutionManagement.CanExecuteFetch()) return;
            var webId = 20;
            var startDate = DateTime.Now.Date.AddDays(-4);
            var endDate = DateTime.Now.Date.AddDays(-4);
            var webParameter = BaseWebIneractive.GetWebNavigation(webId,
                startDate,
                endDate);
            var interactive = new CollinWebInteractive(webParameter, startDate, endDate);
            var result = interactive.Fetch();
            Assert.IsNotNull(result);
            ExcelWriter.WriteToExcel(result);
        }

        [TestMethod]
        [TestCategory("collin.county.actions")]
        [TestCategory("Web.Integration")]
        public void CanGetCriminalCasesFromCollinInteractive()
        {
            if (!ExecutionManagement.CanExecuteFetch()) return;
            // manipulate parameters to setup a criminal search
            const string jsFile = @"D:\Alpha\LegalLead\Thompson.RecordSearch.Utility.Tests\Json\collin-criminal-case-parameter.json";
            var webId = 20;
            var startDate = DateTime.Now.Date.AddDays(-4);
            var endDate = DateTime.Now.Date.AddDays(-4);
            var webParameter = BaseWebIneractive.GetWebNavigation(webId,
                startDate,
                endDate);
            webParameter = CreateOrLoadWebParameter(webParameter, jsFile);
            var interactive = new CollinWebInteractive(webParameter, startDate, endDate);
            var result = interactive.Fetch();
            Assert.IsNotNull(result);
            ExcelWriter.WriteToExcel(result);
        }


        [TestMethod]
        [TestCategory("collin.county.actions")]
        [TestCategory("Web.Integration")]
        public void CanGetProbateCasesFromCollinInteractive()
        {
            if (!ExecutionManagement.CanExecuteFetch()) return;
            // manipulate parameters to setup a criminal search
            const string jsFile = @"D:\Alpha\LegalLead\Thompson.RecordSearch.Utility.Tests\Json\collin-probate-case-parameter.json";
            var webId = 20;
            var startDate = DateTime.Now.Date.AddDays(-4);
            var endDate = DateTime.Now.Date.AddDays(-4);
            var webParameter = BaseWebIneractive.GetWebNavigation(webId,
                startDate,
                endDate);
            webParameter = CreateOrLoadWebParameter(webParameter, jsFile);
            var interactive = new CollinWebInteractive(webParameter, startDate, endDate);
            var result = interactive.Fetch();
            Assert.IsNotNull(result);
            ExcelWriter.WriteToExcel(result);
        }

        private static WebNavigationParameter CreateOrLoadWebParameter(WebNavigationParameter webParameter, string jsFile)
        {
            // get key name 
            // var cultureInfo = System.Globalization.CultureInfo.CurrentCulture;
            if (!File.Exists(jsFile)) return webParameter;
            var keyName = string.Concat(Path.GetFileName(jsFile), ".overwrite");
            var key = ConfigurationManager.AppSettings[keyName] ?? string.Empty;
            var createNewFile = key.Equals("true", StringComparison.CurrentCultureIgnoreCase);
            if (createNewFile) { CreateJsFile(webParameter, jsFile); }
            // load parameter from json
            return ReadJsFile(jsFile);
        }

        private static void CreateJsFile(WebNavigationParameter webParameter, string jsFile)
        {
            using (var writer = new StreamWriter(jsFile))
            {
                writer.Write(
                Newtonsoft.Json.JsonConvert.SerializeObject(webParameter));
            }
        }

        private static WebNavigationParameter ReadJsFile(string jsFile)
        {
            using (var reader = new StreamReader(jsFile))
            {
                var content = reader.ReadToEnd();
                var webParameter =
                    Newtonsoft.Json.JsonConvert
                    .DeserializeObject<WebNavigationParameter>(content);
                return webParameter;
            }
        }
    }
}
