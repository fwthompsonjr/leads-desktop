using OpenQA.Selenium;
using System;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class FortBendGetLinkCollection : BaseFortBendSearchAction
    {
        public override int OrderId => 70;
        public IJavaScriptExecutor ExternalExecutor { get; set; } = null;
        public override object Execute()
        {
            var js = JsScript;
            var executor = ExternalExecutor ?? GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            js = VerifyScript(js);

            return executor.ExecuteScript(js);
        }

        protected override string ScriptName { get; } = "find case detail links";
    }
}