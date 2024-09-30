using LegalLead.PublicData.Search.Classes;
using OpenQA.Selenium;
using System;

namespace LegalLead.PublicData.Search.Util
{
    public class DallasBeginNavigation : IDallasAction
    {
        public IWebDriver Driver { get; set; }
        public DallasAttendedProcess Parameters { get; set; }

        public object Execute()
        {
            throw new NotImplementedException();
        }
    }
}
