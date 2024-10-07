using OpenQA.Selenium;
using System;
using System.Globalization;

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
            var content = TryFetchCaseData(executor, js);
            return content;
        }
        private static string TryFetchCaseData(IJavaScriptExecutor executor, string js)
        {
            const string errtext = "--error--";
            try
            {
                var content = executor.ExecuteScript(js);
                return Convert.ToString(content, CultureInfo.CurrentCulture);
            }
            catch
            {
                return errtext;
            }
        }

        protected override string ScriptName { get; } = "get case style";
    }
}