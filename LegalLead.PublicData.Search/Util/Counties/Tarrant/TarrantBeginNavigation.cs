using System;

namespace LegalLead.PublicData.Search.Util
{
    public class TarrantBeginNavigation : BaseTarrantSearchAction
    {
        public override int OrderId => 10;
        public override object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(ERR_DRIVER_UNAVAILABLE);
            return BeginNavigation(Driver);
        }
    }
}
