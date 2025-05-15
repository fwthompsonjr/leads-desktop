using OpenQA.Selenium;
using System;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Util
{
    public class HarrisGetRecordCount : BaseHarrisSearchAction
    {
        public override int OrderId => 35;
        public override object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(ERR_DRIVER_UNAVAILABLE);


            var find = By.Id("ctl00_ContentPlaceHolder1_lblCount");
            var canExecute = WaitForExists(Driver, find);
            if (!canExecute) return 0;

            int found = GetSearchRecordCount(Driver);
            if (found > 0)
            {
                Interactive.EchoProgess(0, found, 1, $"Found {found} records.");
            }
            return found;
        }
    }
}