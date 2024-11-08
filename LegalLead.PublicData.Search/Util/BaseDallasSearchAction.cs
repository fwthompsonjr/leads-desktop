using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BaseDallasSearchAction : ICountySearchAction
    {
        public virtual int OrderId => 0;
        protected virtual string ScriptName { get; }
        public IWebDriver Driver { get; set; }
        public DallasSearchProcess Parameters { get; set; }

        public virtual object Execute() { return null; }

        public virtual IJavaScriptExecutor GetJavaScriptExecutor()
        {
            if (Driver is IJavaScriptExecutor exec) return exec;
            return null;
        }

        public void MaskUserName()
        {
            var executor = GetJavaScriptExecutor();
            if (executor == null) return;
            var script = string.Join(Environment.NewLine, maskWelcome);
            executor.ExecuteScript(script);
        }


        protected void WaitForNavigation()
        {
            const string request = "return document.readyState";
            const string response = "complete";
            var driver = Driver;
            var jsexec = GetJavaScriptExecutor();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30)) { PollingInterval = TimeSpan.FromMilliseconds(500) };
            wait.Until(driver1 => jsexec.ExecuteScript(request).Equals(response));
        }

        protected virtual string JavaScriptContent { get; set; } = null;
        protected string JsScript
        {
            get
            {
                if (!string.IsNullOrEmpty(JavaScriptContent)) return JavaScriptContent;
                JavaScriptContent = GetJsScript(ScriptName);
                return JavaScriptContent;
            }
        }

        [ExcludeFromCodeCoverage]
        protected static string VerifyScript(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException(Rx.ERR_SCRIPT_MISSING);
            return source;
        }

        protected static string GetJsScript(string keyname)
        {
            lock (lockObject)
            {
                var obj = DallasScriptHelper.ScriptCollection;
                var exists = obj.TryGetValue(keyname, out var js);
                if (!exists) return string.Empty;
                return js;
            }
        }
        private static readonly object lockObject = new();
        private static readonly List<string> maskWelcome = new()
        {
                "var drop = document.getElementById('dropdownMenu1');",
                "if ( null != drop ) {",
                "var arr = Array.prototype.slice.call( drop.getElementsByTagName('span'), 0 );",
                "var spn = arr.find(a => a.innerText.indexOf('Welcome') >= 0); ",
                "if ( null != spn) { spn.innerText = 'Welcome, User'}",
                "}"
        };
    }
}
