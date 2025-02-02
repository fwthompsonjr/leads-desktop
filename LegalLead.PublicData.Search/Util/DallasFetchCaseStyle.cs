using OpenQA.Selenium;
using System;
using System.Globalization;
using System.Threading;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasFetchCaseStyle : BaseDallasSearchAction
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
            string content = string.Empty;
            js = VerifyScript(js);
            var retries = 5;
            var intervals = new int[] { 2000, 2000, 2000, 1500, 1000, 500, 500 };
            while (retries > 0) {
                var waitms = intervals[retries];
                content = ReadCaseDetail(js, executor, uri);
                if (!string.IsNullOrEmpty(content) && !content.Equals(errtext)) {
                    break;
                }
                Thread.Sleep(waitms);
                retries--;
            }
            
            return content;
        }

        private string ReadCaseDetail(string js, IJavaScriptExecutor executor, Uri uri)
        {
            try
            {
                Driver.Navigate().GoToUrl(uri);
                WaitForNavigation();

                var content = TryFetchCaseData(executor, js);
                return content;
            }
            catch (Exception)
            {
                return errtext;
            }
        }

        private static string TryFetchCaseData(IJavaScriptExecutor executor, string js)
        {
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
private const string errtext = "--error--";
        protected override string ScriptName { get; } = "get case style";
    }
}