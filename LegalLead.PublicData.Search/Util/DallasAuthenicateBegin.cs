﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics.CodeAnalysis;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasAuthenicateBegin : DallasBaseExecutor
    {
        protected readonly ICountyCodeReader _reader;
        public DallasAuthenicateBegin(ICountyCodeReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            _reader = reader;
        }
        public override int OrderId => 4;
        public override object Execute()
        {
            if (string.IsNullOrEmpty(_credential))
                _credential = GetCountyCode(_reader);

            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            var destination = NavigationUri;

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            Uri uri = GetUri(destination);
            Driver.Navigate().GoToUrl(uri);
            if (string.IsNullOrEmpty(_credential)) return false;

            js = VerifyScript(js);
            executor.ExecuteScript(js);
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(500),
                };
                wait.Until(d =>
                {
                    var uid = d.TryFindElement(By.Id("UserName"));
                    return uid != null;
                });
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        protected string _credential;
        protected static string GetCountyCode(ICountyCodeReader reader)
        {
            try
            {
                var dallasId = (int)SourceType.DallasCounty;
                if (reader == null) return string.Empty;
                return reader.GetCountyCode(dallasId);
            }
            catch
            {
                return string.Empty;
            }
        }

        [ExcludeFromCodeCoverage]
        private static Uri GetUri(string destination)
        {
            if (!Uri.TryCreate(destination, UriKind.Absolute, out var uri))
                throw new ArgumentException(Rx.ERR_URI_MISSING);
            return uri;
        }

        private static string navigationUri = null;
        private static string NavigationUri
        {
            get
            {
                if (!string.IsNullOrEmpty(navigationUri)) return navigationUri;
                navigationUri = GetNavigationUri();
                return navigationUri;
            }
        }

        private static string GetNavigationUri()
        {
            var obj = DallasScriptHelper.NavigationSteps;
            var item = obj.Find(x => x.ActionName == "navigate");
            if (item == null) return string.Empty;
            return item.Locator.Query;
        }

        protected override string ScriptName { get; } = "login process 01";
    }
}