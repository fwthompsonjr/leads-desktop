using System;
using System.Globalization;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasFetchPersonAddress : DallasBaseExecutor
    {
        public override int OrderId => 80;

        public DallasCaseItemDto Dto { get; set; }

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