using System;
using System.Threading;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class HidalgoNoCountVerification : BaseHidalgoSearchAction
    {
        public override int OrderId => 45;

        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);



            js = VerifyScript(js);
            var response = executor.ExecuteScript(js);
            if (response is bool noCount) return noCount;
            var retries = 5;
            while (retries > 0)
            {
                response = executor.ExecuteScript(js);
                if (response is bool rsp) return rsp;
                Thread.Sleep(500);
                retries--;
            }
            return false;
        }
        protected override string ScriptName { get; } = "find no match";

    }
}