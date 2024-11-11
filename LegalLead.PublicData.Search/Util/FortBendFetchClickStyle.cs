using LegalLead.PublicData.Search.Common;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
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
            var links = this.GetCaseNumbers();
            if (links == null || links.Count == 0) return JsonConvert.SerializeObject(alldata);
            var dataset = this.GetCaseItems(links, GetDto, js);
            if (dataset == null || dataset.Count == 0) return JsonConvert.SerializeObject(alldata);
            dataset.ForEach(d =>
            {
                if (d != null) alldata.Add(d);
            });

            return JsonConvert.SerializeObject(alldata);
        }

        private bool ElementWait(By locator)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5)) { PollingInterval = TimeSpan.FromMilliseconds(300) };
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