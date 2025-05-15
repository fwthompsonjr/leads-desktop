using OpenQA.Selenium;
using System;

namespace LegalLead.PublicData.Search.Util
{
    public class HarrisSetSearchContext : BaseHarrisSearchAction
    {
        public override int OrderId => 15;
        public override object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(ERR_DRIVER_UNAVAILABLE);
            var find = By.Id("ctl00_ContentPlaceHolder1_ddlCourt");
            var canExecute = WaitForExists(Driver, find);
            if (!canExecute) return false;
            return SetContext(Driver);
        }
    }
}