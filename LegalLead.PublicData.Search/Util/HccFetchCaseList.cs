using LegalLead.PublicData.Search.Interfaces;
using System;
using System.Globalization;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class HccFetchCaseList : BaseBexarSearchAction
    {
        public override int OrderId => 60;
        public IHccReadingService HccService { get; set; } = null;
        public override object Execute()
        {
            if (Parameters == null || string.IsNullOrEmpty(Parameters.StartDate))
                throw new NullReferenceException(Rx.ERR_START_DATE_MISSING);
            var isDate = DateTime.TryParse(Parameters.StartDate,
                CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal,
                out var dateStart);
            if (!isDate)
                throw new NullReferenceException(Rx.ERR_START_DATE_MISSING);
            if (HccService == null) return "[]";
            var dataset = HccService.Find(dateStart);
            return dataset.ToJsonString();
        }
    }
}