using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;
using System;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class TravisRequestCaptcha : BaseRequestCaptcha, ITravisSearchAction
    {
        public object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            return GetPromptResponse();
        }
        public int OrderId => 20;

        public IWebDriver Driver { get; set; }
        public TravisSearchProcess Parameters { get; set; }
        public IWebInteractive Interactive { get; set; }
        protected override IWebDriver WebDriver => Driver;

        public bool IsPostSearch => false;
    }
}