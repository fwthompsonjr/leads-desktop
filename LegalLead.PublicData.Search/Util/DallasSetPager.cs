using System;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasSetPager : BaseDallasSearchAction
    {
        public override int OrderId => 50;
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            js = VerifyScript(js);
            executor.ExecuteScript(js);
            return true;
        }

        protected override string ScriptName { get; } = "select max rows per page";
    }
}