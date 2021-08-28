using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Web;

namespace Thompson.RecordSearch.Utility.Tests
{
    [TestClass]
    public class TarrantCountyNavigationTests : TestingBase
    {
        #region Unit Tests

        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        public void CanGetElementActions()
        {
            var actions = ElementActions;
            Assert.IsNotNull(actions);
            Assert.IsTrue(actions.Any());
        }

        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        public void CanGetAndMapSettings()
        {
            var startingDate = DateTime.Now.AddDays(-2);
            var endingDate = DateTime.Now.AddDays(-2);
            var settings = SettingsManager
                .GetNavigation().Find(x => x.Id == 10);
            var datelist = new List<string> { "startDate", "endDate" };
            var keys = settings.Keys.FindAll(s => datelist.Contains(s.Name));
            keys.First().Value = startingDate.ToString("MM/dd/yyyy");
            keys.Last().Value = endingDate.ToString("MM/dd/yyyy");
            Assert.IsNotNull(settings);

        }

        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        public void CanGetTarrtantCountInstructions()
        {
            var fileName = "tarrantCountyMapping_1";

            IWebDriver driver = GetDriver();
            var assertion = new ElementAssertion(driver);
            var sources = fileName.Split(',').ToList();
            try
            {

                var steps = new List<NavigationStep>();
                sources.ForEach(s => steps.AddRange(GetAppSteps(s).Steps));

                var startingDate = DateTime.Now.AddDays(-2);
                var endingDate = DateTime.Now.AddDays(-2);

                // driver = GetAuthenicatedDriver(driver);
                ElementActions.ForEach(x => x.GetAssertion = assertion);
                ElementActions.ForEach(x => x.GetWeb = driver);
                Thread.Sleep(1500);
                foreach (var item in steps)
                {
                    // if item action-name = 'set-text'
                    var actionName = item.ActionName;
                    if (item.ActionName.Equals("set-text"))
                    {
                        if (item.DisplayName.Equals("startDate")) item.ExpectedValue = startingDate.Date.ToString("MM/dd/yyyy");
                        if (item.DisplayName.Equals("endDate")) item.ExpectedValue = endingDate.Date.ToString("MM/dd/yyyy");
                    }
                    var action = ElementActions.FirstOrDefault(x => x.ActionName.Equals(item.ActionName));
                    if (action == null) continue;
                    action.Act(item);
                    if (actionName.Equals("get-table-html") && !string.IsNullOrEmpty(action.OuterHtml))
                    {
                        // create a list of hlinkdatarows from table
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                driver.Close();
                driver.Quit();

                KillProcess("chromedriver");

            }
        }

        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        public void CanGetFromJsonInteractive()
        {
            if (!ExecutionManagement.CanExecuteFetch()) return;
            var webParameter = BaseWebIneractive.GetWebNavigation(10,
                DateTime.Now.Date.AddDays(-4),
                DateTime.Now.Date.AddDays(-4));
            var interactive = new TarrantWebInteractive(webParameter);
            var result = interactive.Fetch();
            Console.WriteLine("DataFile:= {0}", result.Result);
            Assert.IsNotNull(result);
            ExcelWriter.WriteToExcel(result);

        }


        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        [TestCategory("Web.Integration")]
        public void CanGetAndWriteFromJsonInteractive()
        {
            if (!ExecutionManagement.CanExecuteFetch()) return;
            var webParameter = BaseWebIneractive.GetWebNavigation(10,
                DateTime.Now.Date.AddDays(0),
                DateTime.Now.Date.AddDays(0));
            var interactive = new TarrantWebInteractive(webParameter);
            var result = interactive.Fetch();
            Assert.IsNotNull(result);
            ExcelWriter.WriteToExcel(result);

        }

        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        public void CanNavigateSteps1()
        {

            var fileName = "tarrantCountyMapping_1";
            var steps = GetAppSteps(fileName);
            Assert.IsNotNull(steps);
            Assert.IsTrue(steps.Steps.Any());
        }


        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        public void CanNavigateSteps2()
        {

            var fileName = "tarrantCountyMapping_2";
            var steps = GetAppSteps(fileName);
            Assert.IsNotNull(steps);
            Assert.IsTrue(steps.Steps.Any());
        }

        [TestMethod]
        [TestCategory("tarrant.county.versioning")]
        public void CanInitVersionProvider()
        {
            var provider = new VersionNameProvider();
            Assert.IsNotNull(provider);
            Assert.IsFalse(string.IsNullOrEmpty(provider.Name));
            Assert.IsFalse(string.IsNullOrEmpty(VersionNameProvider.FileVersion));
            Assert.IsNotNull(VersionNameProvider.VersionNames);
        }

        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        public void CanGetDropDownCourts()
        {
            var filex = TarrantWebInteractive.GetComboBoxValues();
            Assert.IsNotNull(filex);
        }
        #endregion



        #region Element Navigation Helpers

        private List<IElementActionBase> elementActions;

        private List<IElementActionBase> ElementActions
        {
            get { return elementActions ?? (elementActions = GetElementActions()); }
        }

        private List<IElementActionBase> GetElementActions()
        {
            var container = ActionElementContainer.GetContainer;
            return container.GetAllInstances<IElementActionBase>().ToList();
        }

        private NavigationInstructionDto GetAppSteps(string suffix = "")
        {
            const string dataFormat = @"{0}\xml\{1}.json";
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(dataFormat,
                appDirectory,
                suffix);
            Assert.IsTrue(File.Exists(dataFile));
            var data = File.ReadAllText(dataFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<NavigationInstructionDto>(data);

        }
        #endregion
    }
}
