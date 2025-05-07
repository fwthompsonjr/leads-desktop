using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Enumerations;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    public class BaseTarrantAction
    {
        protected readonly ITarrantConfigurationBoProvider BoProvider
            = ActionTarrantContainer.GetContainer.GetInstance<ITarrantConfigurationBoProvider>();
        protected string HumanScriptJs => humanScript ??= GetHumanScriptJs();
        public Func<bool> PromptUser { get; set; }

        protected bool IsCaptchaRequested(IWebDriver driver)
        {
            if (driver is not IJavaScriptExecutor exec) return false;
            var response = exec.ExecuteScript(HumanScriptJs);
            if (response is not string result) return false;
            var model = JsonConvert.DeserializeObject<HumanResponseModel>(result);
            if (model is null) return false;
            return model.HasCaptcha;
        }
        protected bool SetContext(IWebDriver driver, TarrantReadMode readMode, int locationId)
        {
            const string scriptName = "set-search-context";
            var searchType = BoProvider.GetSearchName(readMode);
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
            return ExecuteScriptWithWait(driver, exec, jscript);
        }

        protected void ReadPersonDetails(IWebDriver driver, List<CaseItemDto> cases)
        {
            const string scriptName = "fetch-search-address-details";
            if (driver is not IJavaScriptExecutor exec) return;
            var jscript = BoProvider.GetJs(scriptName);
            cases.ForEach(c =>
            {
                ReadPersonDetails(driver, c, exec, jscript);
            });
        }

        private static void ReadPersonDetails(IWebDriver driver, CaseItemDto c, IJavaScriptExecutor exec, string jscript)
        {
            try
            {

                var uri = c.Href;
                if (!Uri.TryCreate(uri, UriKind.Absolute, out var url)) return;

                var currentUri = driver.Url;
                driver.Navigate().GoToUrl(url);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(500),
                };
                wait.Until(d => !d.Url.Equals(currentUri, StringComparison.OrdinalIgnoreCase));
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
        }

        protected List<CaseItemDto> ReadCaseItems(IWebDriver driver, TarrantReadMode readMode)
        {
            var list = new List<CaseItemDto>();
            string scriptName = readMode switch {
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
                if (IsCaptchaRequested(driver)) { return PromptUser(); }
                return true;
            }
            catch
            {
                return false;
            }
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
        private string GetHumanScriptJs()
        {
            return BoProvider.GetJs(HumanScriptName);
        }

        private string humanScript;
        private const string HumanScriptName = "is-human-page";
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
    }
}
