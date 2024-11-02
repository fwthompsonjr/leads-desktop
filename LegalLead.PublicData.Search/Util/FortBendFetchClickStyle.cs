using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class FortBendFetchClickStyle : BaseFortBendSearchAction
    {
        public override int OrderId => 65;

        public override object Execute()
        {
            var executor = GetJavaScriptExecutor();

            if (Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            var js = JsScript;
            js = VerifyScript(js);
            var alldata = new List<CaseItemDto>();
            var collector = new FortBendGetLinkCollection()
            {
                Driver = Driver,
                ExternalExecutor = executor,
                Parameters = Parameters
            };
            var navigator = new FortBendGetLinkCollectionItem()
            {
                Driver = Driver,
                ExternalExecutor = executor,
                Parameters = Parameters
            };
            var collection = collector.Execute();
            var locator = By.XPath("//div[@class='ssCaseDetailCaseNbr']");
            if (collection is not string items) return JsonConvert.SerializeObject(alldata);
            var links = JsonConvert.DeserializeObject<List<string>>(items);
            if (links == null || links.Count == 0)
                return JsonConvert.SerializeObject(alldata);
            links.ForEach(link =>
            {
                var id = links.IndexOf(link);
                navigator.LinkItemId = id;
                navigator.Execute();
                if (ElementWait(locator))
                {
                    var person = GetDto(executor.ExecuteScript(js));
                    if (person != null) alldata.Add(person);
                }
            });

            return JsonConvert.SerializeObject(alldata);
        }

        private bool ElementWait(By locator)
        {
            try
            {
                var wait = new WebDriverWait(Driver, PageWaitTimeSpan) { PollingInterval = PageWaitPoolingInterval };
                wait.Until(w =>
                {
                    return w.TryFindElement(locator) != null;
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected override string ScriptName { get; } = "get person address";

        private static CaseItemDto GetDto(object data)
        {
            if (data is not string json) return null;
            var temp = JsonConvert.DeserializeObject<GetItemDto>(json);
            if (temp is null) return null;
            if (string.IsNullOrEmpty(temp.CaseNo)) return null;
            return new CaseItemDto
            {
                Address = temp.Address,
                CaseNumber = temp.CaseNo,
                PartyName = temp.Name
            };
        }

        private sealed class GetItemDto
        {
            [JsonProperty("caseNo")] public string CaseNo { get; set; } = string.Empty;
            [JsonProperty("name")] public string Name { get; set; } = string.Empty;
            [JsonProperty("address")] public string Address { get; set; } = string.Empty;
        }
    }
}