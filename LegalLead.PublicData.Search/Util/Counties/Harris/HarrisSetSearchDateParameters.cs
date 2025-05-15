using OpenQA.Selenium;
using System;
using System.Globalization;

namespace LegalLead.PublicData.Search.Util
{
    public class HarrisSetSearchDateParameters : BaseHarrisSearchAction
    {
        public override int OrderId => 25;
        public override object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(ERR_DRIVER_UNAVAILABLE);
            if (string.IsNullOrEmpty(Parameters.StartDate) || !DateTime.TryParse(Parameters.StartDate,
                CultureInfo.CurrentCulture, out var startDt))
                throw new NullReferenceException(ERR_START_DATE_MISSING);

            if (string.IsNullOrEmpty(Parameters.EndingDate) || !DateTime.TryParse(Parameters.EndingDate,
                CultureInfo.CurrentCulture, out var endingDt))
                throw new NullReferenceException(ERR_END_DATE_MISSING);

            var find = By.Id("ctl00_ContentPlaceHolder1_ddlCourt");
            var canExecute = WaitForExists(Driver, find);
            if (!canExecute) return false;

            return SetDateParameters(Driver, startDt, endingDt);
        }
    }
}