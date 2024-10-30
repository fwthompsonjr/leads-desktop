using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BexarSetNoCountVerification : BaseBexarSearchAction
    {
        public override int OrderId => 45;

        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            
            if (!WaitForPager()) return false;

            js = VerifyScript(js);
            var dto = GetPagingDto(executor, js);
            if (dto.IsNoCount) return true;
            var retries = 5;
            while (retries > 0)
            {
                dto = GetPagingDto(executor, js);
                if (dto.IsNoCount) break;
                Thread.Sleep(500);
                retries--;
            }
            return dto.IsNoCount;
        }
        private bool WaitForPager()
        {
            try
            {
                const int mxwait = 10;
                IWebElement found = null;
                var selection = By.Id("noResults");
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(mxwait))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(750)
                };
                wait.Until(d => CheckForElement(d, selection, out found));
                return found != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        private static bool CheckForElement(IWebDriver d, By finder, out IWebElement element)
        {
            element = d.TryFindElement(finder);
            return element != null;
        }
        private static JsPagingDto GetPagingDto(IJavaScriptExecutor jsobject, string jsscript)
        {
            try
            {
                var response = jsobject.ExecuteScript(jsscript);
                if (response is not string json) return new();
                var obj = JsonConvert.DeserializeObject<JsPagingDto>(json) ?? new();
                return obj;
            }
            catch
            {
                return new();
            }
        }
        protected override string ScriptName { get; } = "check for no count";

        private class JsPagingDto
        {
            [JsonProperty("status")]
            public bool IsNoCount { get; set; }
        }
    }
}