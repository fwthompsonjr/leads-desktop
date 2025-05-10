using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    public class TarrantFetchPersonDetail : BaseTarrantSearchAction
    {
        public override int OrderId => 75;
        public List<CaseItemDto> Items { get; private set; } = [];
        public override object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(ERR_DRIVER_UNAVAILABLE);
            Web = Interactive;
            var alldata = new List<CaseItemDto>(Items);
            ReadPersonDetails(Driver, alldata);
            return JsonConvert.SerializeObject(alldata);
        }
    }
}
