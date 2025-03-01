using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Extensions;
using System.Runtime;
using LegalLead.PublicData.Search.Interfaces;
using System.Threading;
using OpenQA.Selenium.Support.UI;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasNavigateSearch : BaseDallasSearchAction
    {
        public override int OrderId => 40;
        private GetSettingResponse SettingResponse { get; set; } = null;
        public ICaseTypeIterator CaseTypeIterator { get; set; } = null;
        public override object Execute()
        {
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            const string controlName = "btnSSSubmit";
            var errmessage = string.Format(CultureInfo.CurrentCulture, "Automation failed to click submit button '{0}'", controlName);
            var locator = By.Id(controlName);
            var button = Driver.FindElement(locator);
            GetSettings();
            var count = 1;
            var requests = TryClickingElement(executor, button);
            while (count < 10 && requests < 0)
            {
                requests = TryClickingElement(executor, button);
                count++;
            }
            if (requests < 0) throw new ElementNotInteractableException(errmessage);

            return true;
        }
        public void RemediateSearch()
        {

            if (SettingResponse == null || !SettingResponse.IsValid() || CaseTypeIterator == null)
            {
                return;
            }
            SettingResponse.ResetNavigation(Driver);
        }
        protected static int TryClickingElement(IJavaScriptExecutor jse, IWebElement button)
        {
            const int failed = -1;
            const int success = 1;
            try
            {
                if (jse == null || button == null) return failed;
                jse.ExecuteScript("arguments[0].click()", button);
                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return failed;
            }
        }
        
        protected override string ScriptName { get; } = "click search";

        private void GetSettings()
        {
            var executor = GetJavaScriptExecutor();
            var context = executor.ExecuteScript(getSettingsJs);
            if (context is not string contextObj) return;
            var settings = contextObj.ToInstance<GetSettingResponse>();
            if (settings == null || !settings.IsValid()) return;
            SettingResponse = settings;
        }

        public void RestoreSettings()
        {
            if (SettingResponse == null || !SettingResponse.IsValid() || CaseTypeIterator == null) return;
            SettingResponse.ResetNavigation(Driver);
            var executor = GetJavaScriptExecutor();
            SettingResponse.ControlMap.ForEach(control => {
                var js = $"document.getElementById('{control.Name}').value = '{control.Value}'";
                executor.ExecuteScript(js);
            });
            CaseTypeIterator.SetSearchParameter();
        }

        private static readonly string[] arrGetSettings = [
            "var indexes = ['caseCriteria_SearchCriteria', 'caseCriteria.FileDateStart', 'caseCriteria.FileDateEnd'];",
            "var textBoxValues = indexes.map(function(id) {",
            "   var textBox = document.getElementById(id);",
            "   var textVal = textBox ? textBox.value : '';",
            "   return { name: id, value: textVal };",
            "});",
            "var context = {",
            "  home: 'https://courtsportal.dallascounty.org/DALLASPROD/',",
            "  page: document.location.href,",
            "  controls: JSON.stringify(textBoxValues) }",
            "return JSON.stringify(context)"
            ];
        private static string getSettingsJs = string.Join(Environment.NewLine, arrGetSettings);
        private class GetSettingResponse
        {
            public string Home { get; set; } = string.Empty;
            public string Page { get; set; } = string.Empty;
            public string Controls { get; set; } = string.Empty;
            public List<ControlMap> ControlMap { get; private set; }
            public bool IsValid()
            {
                if (string.IsNullOrWhiteSpace(Home)) return false;
                if (string.IsNullOrWhiteSpace(Page)) return false;
                if (string.IsNullOrWhiteSpace(Controls)) return false;
                if (ControlMap != null && ControlMap.Count > 0) return true;
                var obj = Controls.ToInstance<List<ControlMap>>();
                if (obj == null) return false;
                ControlMap = obj;
                return true;
            }

            public void ResetNavigation(IWebDriver driver)
            {
                WaitForHome(driver);
                WaitForReturn(driver);
            }

            protected void WaitForHome(IWebDriver driver)
            {
                var homeUri = new Uri(Home);
                try
                {
                    Console.WriteLine("Search execution timeout. Retrying...");
                    // Navigate to the initial page
                    driver.Navigate().GoToUrl(homeUri);

                    // Define the expected URL ending
                    string expectedUrlEnding = "Dashboard/29";

                    // Create a WebDriverWait instance with a timeout of 45 seconds and a polling interval of 500ms
                    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(45))
                    {
                        PollingInterval = TimeSpan.FromMilliseconds(500)
                    };
                    // Wait until the URL ends with the expected value
                    wait.Until(d => !d.Url.Contains(expectedUrlEnding));
                }
                catch (Exception ex) { 
                    Debug.WriteLine(ex);
                }
            }

            protected void WaitForReturn(IWebDriver driver)
            {
                var pageUri = new Uri(Page);
                try
                {
                    // Navigate to the initial page
                    driver.Navigate().GoToUrl(pageUri);

                    // Define the expected URL ending
                    string expectedUrlEnding = "Dashboard/29";

                    // Create a WebDriverWait instance with a timeout of 45 seconds and a polling interval of 500ms
                    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(45))
                    {
                        PollingInterval = TimeSpan.FromMilliseconds(500)
                    };
                    // Wait until the URL ends with the expected value
                    wait.Until(d => d.Url.Contains(expectedUrlEnding));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

            }
        }

        private class ControlMap
        {
            public string Name { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
        }
    }

}