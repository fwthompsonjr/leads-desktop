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
            return SetDateParameters(Driver, startDt, endingDt);
        }
    }
}