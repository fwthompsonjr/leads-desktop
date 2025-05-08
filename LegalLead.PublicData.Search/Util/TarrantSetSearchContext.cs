using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Enumerations;
using System;

namespace LegalLead.PublicData.Search.Util
{
    public class TarrantSetSearchContext : BaseTarrantSearchAction
    {
        public override int OrderId => 15;
        public override object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(ERR_DRIVER_UNAVAILABLE);
            var context = new SearchContextParameters(Parameters);
            return SetContext(Driver, context.ReadMode, context.LocationId);
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
            public int LocationId
            {
                get
                {
                    var locationName = _source.CourtLocator;
                    var locationId = locationName switch
                    {
                        _ => 1,
                    };
                    return locationId; // default
                }
            }
        }
    }
}
