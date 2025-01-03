﻿using LegalLead.PublicData.Search.Common;
using OpenQA.Selenium;
using System;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class TravisSetupLocation : TravisSetupOptions
    {
        public override int OrderId => 15;

        public override object Execute()
        {
            var dto = TravisScriptHelper.NavigationSetting;
            var xpath = dto.SearchLinkLocator;
            if (Parameters == null || Driver == null || string.IsNullOrEmpty(xpath))
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            var element = Driver.TryFindElement(By.XPath(xpath)) ?? throw new NullReferenceException();
            var executor = Driver.GetJsExecutor();
            if (executor != null) element.Click();
            else
            {
                executor.ExecuteScript("arguments[0].click()", element);
            }
            WaitForControls(Driver);
            return true;
        }

        protected override string ScriptName { get; } = "set start and end date";
    }
}