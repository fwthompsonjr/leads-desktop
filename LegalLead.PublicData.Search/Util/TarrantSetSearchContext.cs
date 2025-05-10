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
            var locationName = Parameters.UserSelectedCourtType;
            var locationIndex = BoProvider.GetLocationIndex(locationName);
            var actualName = BoProvider.GetLocationName(locationIndex);
            Console.WriteLine("Searching: {0}", actualName);
            return SetContext(Driver, context.ReadMode, locationIndex);
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
