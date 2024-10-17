using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;
using System;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasRequestCaptcha : BaseRequestCaptcha, ICountySearchAction
    {
        public virtual object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            return GetPromptResponse();
        }
        public int OrderId => 20;

        public IWebDriver Driver { get; set; }
        public DallasSearchProcess Parameters { get; set; }

        protected override IWebDriver WebDriver => Driver;
    }
}