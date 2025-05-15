using OpenQA.Selenium;
using System;
using System.Globalization;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class FortBendGetLinkCollectionItem : BaseFortBendSearchAction
    {
        public override int OrderId => 75;
        public int LinkItemId { get; set; }
        public IJavaScriptExecutor ExternalExecutor { get; set; } = null;
        public override object Execute()
        {
            var js = JsScript;
            var executor = ExternalExecutor ?? GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            js = VerifyScript(js);
            var script = js
                .Replace("{0}", LinkItemId.ToString(CultureInfo.CurrentCulture));
            return executor.ExecuteScript(script);
        }

        protected override string ScriptName { get; } = "click case detail links";
    }
}