using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Interfaces;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BaseHarrisAction
    {
        #region Fields

        protected readonly IHarrisCivilConfigurationBoProvider BoProvider
            = ActionHarrisContainer.GetContainer.GetInstance<IHarrisCivilConfigurationBoProvider>();
        protected static string ERR_DRIVER_UNAVAILABLE => Rx.ERR_DRIVER_UNAVAILABLE;
        protected static string ERR_START_DATE_MISSING => Rx.ERR_START_DATE_MISSING;
        protected static string ERR_END_DATE_MISSING => Rx.ERR_END_DATE_MISSING;

        #endregion
        #region Properties

        protected IWebInteractive Web { get; set; }
        #endregion

        #region Protected Methods

        protected bool BeginNavigation(IWebDriver driver)
        {
            var homePage = BoProvider.BasePage;
            try
            {
                return NavigatePage(driver, homePage);
            }
            catch
            {
                return false;
            }

        }
        protected bool SetContext(IWebDriver driver)
        {
            const string scriptName = "set-search-context";
            if (driver is not IJavaScriptExecutor exec) return false;
            var jscript = BoProvider.GetJs(scriptName);
            var response = exec.ExecuteScript(jscript);
            if (response is not string json) return false;
            var rsp = json.ToInstance<SetContextDto>();
            if (rsp is null) return false;
            return rsp.IsOk;
        }

        protected bool SetDateParameters(IWebDriver driver, DateTime startDate, DateTime endingDate)
        {
            const string scriptName = "set-search-date-parameters";
            if (driver is not IJavaScriptExecutor exec) return false;
            var builder = new StringBuilder(BoProvider.GetJs(scriptName));
            builder.Replace("{0}", $"{startDate:d}");
            builder.Replace("{1}", $"{endingDate:d}");
            var jscript = builder.ToString();
            var response = exec.ExecuteScript(jscript);
            if (response is not string json) return false;
            var rsp = json.ToInstance<SetContextDto>();
            if (rsp is null) return false;
            return rsp.IsOk;
        }

        protected bool PostFormValues(IWebDriver driver)
        {
            const string scriptName = "post-form-values";
            if (driver is not IJavaScriptExecutor exec) return false;
            var jscript = BoProvider.GetJs(scriptName);
            var response = exec.ExecuteScript(jscript);
            if (null != response) return false;
            driver.WaitForDocumentReady(exec, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(500));
            return true;
        }

        protected int GetSearchRecordCount(IWebDriver driver)
        {
            const string scriptName = "fetch-search-record-count";
            if (driver is not IJavaScriptExecutor exec) return 0;
            var jscript = BoProvider.GetJs(scriptName);
            var response = exec.ExecuteScript(jscript);
            if (response is not string payload) return 0;
            var dto = payload.ToInstance<GetRecordCountDto>() ?? new();
            return dto.RecordCount;
        }

        protected List<CaseItemDto> ReadCaseItems(IWebDriver driver)
        {
            const string caseScript = "fetch-search-case-style-details";
            const string personScript = "fetch-search-address-details";
            var data = new List<CaseItemDto>();
            if (driver is not IJavaScriptExecutor exec) return data;
            var jscript = BoProvider.GetJs(caseScript);
            var response = exec.ExecuteScript(jscript);
            if (response is not string payload) return data;
            var casesResponse = payload.ToInstance<GetCaseStyleResponse>();
            if (null == casesResponse) return data;
            var casedtos = casesResponse.Data;
            if (casedtos.Count == 0) return data;
            var filingDt = casedtos[0].DateFiled;
            Console.WriteLine($"{filingDt} : Found {casedtos.Count} records");
            casedtos.ForEach(c =>
            {
                data.Add(new()
                {
                    CaseNumber = c.CaseNumber,
                    CaseStyle = c.CaseStyle,
                    CaseType = c.CaseType,
                    Court = c.Court,
                    CourtDate = c.DateFiled,
                    FileDate = c.DateFiled,
                    CaseStatus = c.StatusCode
                });
            });

            Console.WriteLine($"{filingDt} : Reading address details.");
            jscript = BoProvider.GetJs(personScript);
            response = exec.ExecuteScript(jscript);
            if (response is not string jspayload) return data;
            var addressesResponse = jspayload.ToInstance<GetCaseAddressResponse>();
            if (addressesResponse == null) return data;
            var people = addressesResponse.Data;
            people.ForEach(p =>
            {
                var target = data.Find(x => x.CaseNumber == p.CaseNumber);
                if (target != null)
                {
                    target.Address = p.Address;
                    target.PartyName = p.Defendant;
                    target.SetPartyNameFromCaseStyle();
                }
            });
            return data;
        }

        protected bool HasNextPage(IWebDriver driver)
        {
            const string scriptName = "navigate-next-page";
            if (driver is not IJavaScriptExecutor exec) return false;
            var jscript = BoProvider.GetJs(scriptName, "read");
            var response = exec.ExecuteScript(jscript);
            if (response is not string json) return false;
            var nav = json.ToInstance<NavigateNextDto>() ?? new();
            return nav.HasNextPage;
        }

        protected bool GoToNextPage(IWebDriver driver)
        {
            const string scriptName = "navigate-next-page";
            if (driver is not IJavaScriptExecutor exec) return false;
            var jscript = BoProvider.GetJs(scriptName, "execute");
            exec.ExecuteScript(jscript);
            driver.WaitForDocumentReady(exec, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(500));
            return true;
        }
        #endregion

        #region Private Static Methods


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

        #endregion


        #region Classes
        private class SetContextDto
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;

            [JsonProperty("result")]
            public bool IsOk { get; set; }
        }
        private sealed class GetRecordCountDto
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;

            [JsonProperty("result")]
            public bool IsOk { get; set; }

            [JsonProperty("rowCount")]
            public int RecordCount { get; set; }
        }
        private sealed class NavigateNextDto
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;

            [JsonProperty("result")]
            public bool IsOk { get; set; }
            [JsonProperty("hasNextPage")]
            public bool HasNextPage { get; set; }
        }
        private sealed class GetCaseStyleResponse
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("data")]
            public List<HarrisCaseStyleDto> Data { get; set; } = [];
            [JsonProperty("result")]
            public bool IsOk { get; set; }
        }
        private sealed class GetCaseAddressResponse
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("data")]
            public List<HarrisCaseAddressDto> Data { get; set; } = [];
            [JsonProperty("result")]
            public bool IsOk { get; set; }
        }
        private sealed class HarrisCaseStyleDto
        {
            [JsonProperty("caseNumber")]
            public string CaseNumber { get; set; }

            [JsonProperty("caseStyle")]
            public string CaseStyle { get; set; }

            [JsonProperty("caseType")]
            public string CaseType { get; set; }

            [JsonProperty("status")]
            public string StatusCode { get; set; }

            [JsonProperty("court")]
            public string Court { get; set; }

            [JsonProperty("fileDate")]
            public string DateFiled { get; set; }
        }

        private sealed class HarrisCaseAddressDto
        {
            [JsonProperty("caseNumber")]
            public string CaseNumber { get; set; }

            [JsonProperty("defendant")]
            public string Defendant { get; set; }

            [JsonProperty("address")]
            public string Address { get; set; }
        }
        #endregion
    }
}