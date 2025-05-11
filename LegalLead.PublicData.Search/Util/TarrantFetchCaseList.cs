using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Util
{
    public class TarrantFetchCaseList : BaseTarrantSearchAction
    {
        public override int OrderId => 55;
        public override object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(ERR_DRIVER_UNAVAILABLE);
            var alldata = new List<CaseItemDto>();
            var context = new SearchContextParameters(Parameters);
            var current = ReadCaseItems(Driver, context.ReadMode);
            if (current != null && current.Count > 0) { alldata.AddRange(current); }
            this.Interactive.EchoProgess(0, current.Count, 1, $"Found {current.Count} records.");
            return JsonConvert.SerializeObject(alldata);
        }
        private sealed class SearchContextParameters(DallasSearchProcess source)
        {
            private readonly DallasSearchProcess _source = source;
            public TarrantReadMode ReadMode
            {
                get
                {
                    var isCriminal = _source.CourtType.Equals("Criminal");
                    return isCriminal ? TarrantReadMode.Criminal : TarrantReadMode.Civil;
                }
            }
        }
    }
}
