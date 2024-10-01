using System;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasNavigateSearch : DallasBaseExecutor
    {
        public override int OrderId => 4;
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            js = VerifyScript(js);
            executor.ExecuteScript(js);
            WaitForNavigation();
            return true;
        }

        protected override string ScriptName { get; } = "click search";
    }
}