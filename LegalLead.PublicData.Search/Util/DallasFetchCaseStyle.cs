using System;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasFetchCaseStyle : DallasBaseExecutor
    {
        public override int OrderId => 70;

        public string PageAddress { get; set; }

        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            if (string.IsNullOrEmpty(PageAddress))
                throw new NullReferenceException(Rx.ERR_URI_MISSING);
            
            if (!Uri.TryCreate(PageAddress, UriKind.Absolute, out var uri))
                throw new NullReferenceException(Rx.ERR_URI_MISSING);

            Driver.Navigate().GoToUrl(uri);
            WaitForNavigation();

            js = VerifyScript(js);
            var content = executor.ExecuteScript(js);
            return Convert.ToString(content);
        }


        protected override string ScriptName { get; } = "get case style";
    }
}