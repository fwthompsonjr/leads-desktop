using LegalLead.PublicData.Search.Extensions;
using OpenQA.Selenium;
using System;
using System.Globalization;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;

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
            Uri home = null;
            var homePage = GetNavigationUri();
            if (!string.IsNullOrEmpty(homePage)) home = new Uri(homePage);
            string content = string.Empty;
            js = VerifyScript(js);
            var intervals = new int[] { 1000, 2500, 2000, 1500 };
            var retries = intervals.Length - 1;
            while (retries > 0)
            {
                var waitms = intervals[retries];
                content = ReadCaseDetail(js, executor, uri, home);
                if (!string.IsNullOrEmpty(content) && !content.Equals(errtext))
                {
                    break;
                }
                Thread.Sleep(waitms);
                retries--;
            }

            return content;
        }

        private string ReadCaseDetail(string js, IJavaScriptExecutor executor, Uri uri, Uri fallback)
        {
            try
            {
                Driver.NavigateWithRetry(TimeSpan.FromSeconds(10), uri, fallback);
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


        private static string GetNavigationUri()
        {
            var obj = DallasScriptHelper.NavigationSteps;
            var item = obj.Find(x => x.ActionName == "navigate");
            if (item == null) return string.Empty;
            return item.Locator.Query;
        }
    }
}