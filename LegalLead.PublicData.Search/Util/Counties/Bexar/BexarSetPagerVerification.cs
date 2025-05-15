using Newtonsoft.Json;
using System;
using System.Threading;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BexarSetPagerVerification : BaseBexarSearchAction
    {
        public override int OrderId => 55;

        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            js = VerifyScript(js);
            var retries = 30;
            while (retries > 0)
            {
                var response = executor.ExecuteScript(js);
                if (response is not string json)
                {
                    Thread.Sleep(1000);
                    retries--;
                    continue;
                }
                var obj = GetPagingDto(json);
                if (obj.IsValid()) break;
                Thread.Sleep(1000);
                retries--;
            }
            return true;
        }

        private static JsPagingDto GetPagingDto(string json)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<JsPagingDto>(json) ?? new();
                return obj;
            }
            catch
            {
                return new();
            }
        }
        protected override string ScriptName { get; } = "verify page count";

        private class JsPagingDto
        {
            [JsonProperty("expected")]
            public int Expected { get; set; }
            [JsonProperty("actual")]
            public int Actual { get; set; }
            public bool IsValid()
            {
                if (Expected == 0) return false;
                return Expected == Actual;
            }
        }
    }
}