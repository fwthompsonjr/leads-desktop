﻿using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
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
        public IWebInteractive Interactive { get; set; }
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
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(25)) { PollingInterval = TimeSpan.FromMilliseconds(500) };
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

        protected static void TryHideElements(IJavaScriptExecutor jsexec)
        {
            if (jsexec == null) return;
            try
            {
                jsexec.ExecuteScript(GetHideElementJs);
            }
            catch (Exception)
            {
                // no action taken
            }
        }

        protected static bool IsNoCount(IJavaScriptExecutor jsexec)
        {
            if (jsexec == null) return false;
            var obj = jsexec.ExecuteScript(GetNoCountJs);
            if (obj is not bool bNoCount) return false;
            return bNoCount;
        }

        protected static string GetNoCountJs
        {
            get
            {
                if (!string.IsNullOrEmpty(nocountjs)) return nocountjs;
                nocountjs = string.Join(Environment.NewLine, nocountscript);
                return nocountjs;
            }
        }
        protected static string GetHideElementJs
        {
            get
            {
                if (!string.IsNullOrEmpty(hideelementjs)) return hideelementjs;
                var bytes = Properties.Resources.dallas_hide_elements_js;
                hideelementjs = Encoding.UTF8.GetString(bytes);
                return hideelementjs;
            }
        }
        private static string nocountjs = null;
        private static readonly string[] nocountscript = new[]
        {
            "try { ",
            "	const no_case_text = 'no cases match your search'; ",
            "	var ultabs = document.getElementById('ui-tabs-1'); ",
            "	if (undefined == ultabs || null == ultabs) { return false; } ",
            "	var tabtext = ultabs.innerText.trim().toLowerCase(); ",
            "	return tabtext.indexOf(no_case_text) >= 0;	 ",
            "} catch { ",
            "	return false; ",
            "} "
        };
        private static string hideelementjs = null;
    }
}
