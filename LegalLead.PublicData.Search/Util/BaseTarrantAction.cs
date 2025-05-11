using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Enumerations;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BaseTarrantAction
    {
        #region Fields

        protected readonly ITarrantConfigurationBoProvider BoProvider
            = ActionTarrantContainer.GetContainer.GetInstance<ITarrantConfigurationBoProvider>();
        private const string HumanScriptName = "is-human-page";
        protected static string ERR_DRIVER_UNAVAILABLE => Rx.ERR_DRIVER_UNAVAILABLE;
        protected static string ERR_START_DATE_MISSING => Rx.ERR_START_DATE_MISSING;
        protected static string ERR_END_DATE_MISSING => Rx.ERR_END_DATE_MISSING;
        private static string humanScript;
        
        #endregion
        #region Properties

        public Func<bool> PromptUser { get; set; }
        protected string HumanScriptJs => humanScript ??= GetHumanScriptJs();
        protected IWebInteractive Web { get; set; }
        #endregion

        #region Protected Methods


        protected bool IsCaptchaRequested(IWebDriver driver)
        {
            if (driver is not IJavaScriptExecutor exec) return false;
            var response = exec.ExecuteScript(HumanScriptJs);
            if (response is not string result) return false;
            var model = JsonConvert.DeserializeObject<HumanResponseModel>(result);
            if (model is null) return false;
            return model.HasCaptcha;
        }
        protected bool BeginNavigation(IWebDriver driver)
        {
            var homePage = BoProvider.BasePage;
            try
            {
                NavigatePage(driver, homePage);
                return true;
            }
            catch
            {
                return false;
            }

        }
        protected bool SetContext(IWebDriver driver, TarrantReadMode readMode, int locationId)
        {
            const string scriptName = "set-search-context";
            var searchType = $"{(int)readMode}";
            string courtLocation = BoProvider.GetLocationName(locationId);
            if (driver is not IJavaScriptExecutor exec) return false;
            var jscript = BoProvider.GetJs(scriptName, searchType, courtLocation);
            return ExecuteScriptWithWait(driver, exec, jscript);
        }

        protected bool SetDateParameters(IWebDriver driver, DateTime startDate, DateTime endingDate)
        {
            const string scriptName = "set-search-date-parameters";
            if (driver is not IJavaScriptExecutor exec) return false;
            var jscript = BoProvider.GetJs(scriptName, $"{startDate:d}", $"{endingDate:d}");
            exec.ExecuteScript(jscript);
            var finder = By.XPath("//td[@class='ssHeaderTitleBanner']");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15))
            {
                PollingInterval = TimeSpan.FromMilliseconds(500),
            };
            wait.Until(d =>
            {
                try
                {
                    var element = d.TryFindElement(finder);
                    if (element == null || string.IsNullOrEmpty(element.Text)) return false;
                    return element.Text.Contains("Case Records Search Results", StringComparison.OrdinalIgnoreCase);
                }
                catch (Exception)
                {
                    return true;
                }
            });
            if (IsCaptchaRequested(driver)) { return PromptUser(); }
            return true;
        }

        protected void ReadPersonDetails(IWebDriver driver, List<CaseItemDto> cases)
        {
            const string scriptName = "fetch-search-address-details";
            if (driver is not IJavaScriptExecutor exec) return;
            var jscript = BoProvider.GetJs(scriptName);
            var count = cases.Count;
            if (count == 0) return;
            var filingDate = cases[0].FileDate;
            cases.ForEach(c =>
            {
                ReadPersonDetails(driver, c, exec, jscript);
                if (c is CaseItemDtoTraker trk) { trk.IsProcessed = true; }
                var position = cases.IndexOf(c) + 1;
                Web?.EchoProgess(0, count, position, $"{filingDate} : Reading {position} of {count} records.");
            });
        }

        protected List<CaseItemDto> ReadCaseItems(IWebDriver driver, TarrantReadMode readMode)
        {
            var list = new List<CaseItemDto>();
            string scriptName = readMode switch
            {
                TarrantReadMode.Civil => "civil-case-reader",
                TarrantReadMode.Criminal => "criminal-case-reader",
                _ => "civil-case-reader"
            };
            if (driver is not IJavaScriptExecutor exec) return list;
            var jscript = BoProvider.GetJs(scriptName);
            var response = exec.ExecuteScript(jscript);
            if (response is not string json) return list;
            var data = JsonConvert.DeserializeObject<List<TarrantCaseItemDto>>(json) ?? [];
            data.ForEach(d => AppendCaseDetail(d, list));
            return list;
        }

        #endregion
        #region Private Methods

        private bool ExecuteScriptWithWait(IWebDriver driver, IJavaScriptExecutor exec, string jscript)
        {
            if (string.IsNullOrWhiteSpace(jscript)) return false;
            var currentUri = driver.Url;
            try
            {
                exec.ExecuteScript(jscript);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(500),
                };
                wait.Until(d => !d.Url.Equals(currentUri, StringComparison.OrdinalIgnoreCase));
                driver.WaitForDocumentReady(exec, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(500));
                if (IsCaptchaRequested(driver)) { 
                    return PromptUser(); 
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GetHumanScriptJs()
        {
            return BoProvider.GetJs(HumanScriptName);
        }

        #endregion
        
        #region Private Static Methods

        private static void ReadPersonDetails(IWebDriver driver, CaseItemDto c, IJavaScriptExecutor exec, string jscript)
        {
            try
            {

                var uri = c.Href;
                var navigationSuccess = NavigatePage(driver, uri);
                if (!navigationSuccess)
                {
                    // go back
                    driver.Navigate().Back();
                    NavigateByLinkClick(driver, c.CaseNumber);
                }
                var jsresponse = exec.ExecuteScript(jscript);
                AppendPersonDetail(c, jsresponse);
            }
            catch
            {
                // silently continue on errors in person read
            }
        }

        private static void AppendPersonDetail(CaseItemDto c, object jsresponse)
        {
            if (jsresponse is not string json) return;
            var people = JsonConvert.DeserializeObject<List<TarrantPersonDto>>(json);
            if (people == null || people.Count == 0) return;
            var person = people.Find(p =>
            {
                if (string.IsNullOrEmpty(p.Name)) return false;
                return p.Type.Equals("Defendant", StringComparison.OrdinalIgnoreCase);// Defendant 
            }) ?? people[0];
            c.Address = person.Address;
            if (!string.IsNullOrEmpty(c.PartyName)) return;
            if (string.IsNullOrEmpty(person.CaseStyle) || string.IsNullOrEmpty(person.Name)) return;
            c.CaseStyle = person.CaseStyle;
            c.SetPartyNameFromCaseStyle(true);
        }

        private static bool NavigatePage(IWebDriver driver, string targetUrl)
        {
            if (!Uri.TryCreate(targetUrl, UriKind.Absolute, out var url))
                throw new ArgumentOutOfRangeException(nameof(targetUrl));

            var currentUri = driver.Url;
            driver.Navigate().GoToUrl(url);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5))
            {
                PollingInterval = TimeSpan.FromMilliseconds(500),
            };
            wait.Until(d => !d.Url.Equals(currentUri, StringComparison.OrdinalIgnoreCase));
            // check for an error has occurred
            return !driver.Url.Contains("ErrorOccured");
        }

        private static void NavigateByLinkClick(IWebDriver driver, string caseNumber)
        {
            if (driver is not IJavaScriptExecutor executor) return;
            var finder = By.XPath("//a[@style='color: blue']");
            var links = driver.TryFindElements(finder);
            if (links == null || links.Count == 0) return;
            var numbers = links.Select(x => x.Text).ToList();
            var indx = numbers.FindIndex(x => x.Equals(caseNumber));
            if (indx == -1) return;
            var targetLink = links[indx];

            var currentUri = driver.Url;
            executor.ExecuteScript("arguments[0].click()", targetLink);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5))
            {
                PollingInterval = TimeSpan.FromMilliseconds(500),
            };
            wait.Until(d => !d.Url.Equals(currentUri, StringComparison.OrdinalIgnoreCase));
        }

        private static void AppendCaseDetail(TarrantCaseItemDto d, List<CaseItemDto> list)
        {
            var dto = new CaseItemDto
            {
                CaseNumber = d.CaseNumber,
                CaseStyle = d.CaseStyle,
                Court = d.Court,
                FileDate = d.DateFiled,
                CaseType = d.CaseType,
                Href = d.Uri
            };
            dto.SetPartyNameFromCaseStyle();
            list.Add(dto);
        }
        #endregion


        #region Classes

        private sealed class HumanResponseModel
        {
            [JsonProperty("hasCaptcha")] public bool HasCaptcha { get; set; }
        }
        private sealed class TarrantPersonDto
        {
            [JsonProperty("index")]
            public int Index { get; set; }

            [JsonProperty("caseNumber")]
            public string CaseNumber { get; set; }

            [JsonProperty("caseStyle")]
            public string CaseStyle { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("address")]
            public string Address { get; set; }
        }

        #endregion
    }
}
