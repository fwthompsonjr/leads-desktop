using LegalLead.PublicData.Search.Interfaces;
using System;
using System.Globalization;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class HccCountDatabase : BaseWilliamsonSearchAction
    {
        public override int OrderId => 2;
        public IHccCountingService HccService { get; set; } = null;
        public override object Execute()
        {
            if (HccService == null) { return 0; }
            if (Parameters == null || string.IsNullOrEmpty(Parameters.StartDate))
                throw new NullReferenceException(Rx.ERR_START_DATE_MISSING);
            var isDate = DateTime.TryParse(Parameters.StartDate,
                CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal,
                out var dateStart);
            if (!isDate)
                throw new NullReferenceException(Rx.ERR_START_DATE_MISSING);
            return HccService.Count(dateStart);
        }
    }
}