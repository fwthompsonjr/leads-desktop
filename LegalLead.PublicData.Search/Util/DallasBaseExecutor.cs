﻿using LegalLead.PublicData.Search.Classes;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics.CodeAnalysis;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasBaseExecutor : IDallasAction
    {
        public virtual int OrderId => 0;
        protected virtual string ScriptName { get; }
        public IWebDriver Driver { get; set; }
        public DallasAttendedProcess Parameters { get; set; }

        public virtual object Execute() { return null; }

        public virtual IJavaScriptExecutor GetJavaScriptExecutor()
        {
            if (Driver is IJavaScriptExecutor exec) return exec;
            return null;
        }

        protected void WaitForNavigation()
        {
            const string request = "return document.readyState";
            const string response = "complete";
            var driver = Driver;
            var jsexec = GetJavaScriptExecutor();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
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
        private static readonly object lockObject = new object();
    }
}