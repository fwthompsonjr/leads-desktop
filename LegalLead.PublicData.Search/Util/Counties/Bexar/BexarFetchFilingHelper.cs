using Newtonsoft.Json;
using System;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BexarFetchFilingHelper : BaseBexarSearchAction
    {
        public override int OrderId => 75;
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);


            js = VerifyScript(js);
            var response = executor.ExecuteScript(js);
            if (response is string json) return json;
            return string.Empty;
        }

        public CaseItemDto GetAddress()
        {
            try
            {
                var js = Execute();
                if (js is not string json) return null;
                var dto = JsonConvert.DeserializeObject<FetchNameDto>(json);
                if (dto == null) return null;
                return new CaseItemDto { PartyName = dto.Name, Address = dto.Address, Court = dto.Court };
            }
            catch
            {
                return null;
            }
        }
        protected override string ScriptName { get; } = "get defendant address";
        private class FetchNameDto
        {
            public string Name { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string Court { get; set; } = string.Empty;
        }
    }
}