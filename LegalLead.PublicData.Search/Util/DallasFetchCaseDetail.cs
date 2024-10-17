using System;
using System.Globalization;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasFetchCaseDetail : BaseDallasSearchAction
    {
        public override int OrderId => 60;
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            js = VerifyScript(js);
            var content = executor.ExecuteScript(js);
            return Convert.ToString(content, CultureInfo.CurrentCulture);
        }

        protected override string ScriptName { get; } = "get case list";
    }
}