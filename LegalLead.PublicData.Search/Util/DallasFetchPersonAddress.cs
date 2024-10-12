using System;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasFetchPersonAddress : BaseDallasSearchAction
    {
        public override int OrderId => 80;

        public CaseItemDto Dto { get; set; }

        public override object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            if (Dto == null)
                throw new NullReferenceException(Rx.ERR_URI_MISSING);

            return string.Empty;
        }
    }
}