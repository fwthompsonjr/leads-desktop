using System;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class GraysonNavigateCourtPage : BaseGraysonSearchAction
    {
        public override int OrderId => 15;

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

        protected override string ScriptName { get; } = "nav to search screen";

    }
}