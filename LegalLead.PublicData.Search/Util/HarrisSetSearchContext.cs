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
            return SetContext(Driver);
        }
    }
}