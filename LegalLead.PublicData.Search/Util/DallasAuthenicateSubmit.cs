﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasAuthenicateSubmit : DallasAuthenicateBegin
    {
        public DallasAuthenicateSubmit(ICountyCodeReader reader) : base(reader)
        {
        }

        public override int OrderId => 6;
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            if (string.IsNullOrEmpty(_credential))
                _credential = GetCountyCode(_reader);

            if (string.IsNullOrEmpty(_credential)) return false;
            if (!_credential.Contains("|")) return false;
            var currentTitle = Driver.Title;
            var sustitutes = _credential.Split('|');
            js = VerifyScript(js);
            js = js.Replace("{0}", sustitutes[0]).Replace("{1}", sustitutes[1]);
            executor.ExecuteScript(js);
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(500),
                };
                wait.Until(d =>
                {
                    var nextTitle = d.Title;
                    return nextTitle != currentTitle;
                });
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        protected override string ScriptName { get; } = "login process 02";
    }
}