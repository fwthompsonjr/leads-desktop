using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BaseElPasoSearchAction : ICountySearchAction
    {
        public virtual int OrderId => 0;
        protected virtual string ScriptName { get; }
        protected virtual TimeSpan PageWaitTimeSpan => TimeSpan.FromSeconds(30);
        protected virtual TimeSpan PageWaitPoolingInterval => TimeSpan.FromSeconds(5);
        public IWebDriver Driver { get; set; }
        public DallasSearchProcess Parameters { get; set; }
        public IWebInteractive Interactive { get; set; }
        public virtual object Execute() { return null; }

        public virtual IJavaScriptExecutor GetJavaScriptExecutor()
        {
            return Driver.GetJsExecutor();
        }


        protected static string NavigationUri()
        {
            return _navUri;
        }

        protected void WaitForNavigation()
        {
            var jsexec = GetJavaScriptExecutor();
            Driver.WaitForDocumentReady(jsexec, PageWaitTimeSpan, PageWaitPoolingInterval);
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
                var exists = _collection.TryGetValue(keyname, out var js);
                if (!exists) return string.Empty;
                return js;
            }
        }
        private static readonly object lockObject = new();
        private static readonly string _navUri = ElPasoScriptHelper.GetNavigationUri;
        private static readonly Dictionary<string, string> _collection = ElPasoScriptHelper.ScriptCollection;
    }
}