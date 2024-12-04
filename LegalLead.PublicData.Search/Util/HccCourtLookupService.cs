using LegalLead.PublicData.Search.Common;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Db;

namespace LegalLead.PublicData.Search.Util
{
    public static class HccCourtLookupService
    {
        public static string GetAddress(string courtType, string court)
        {
            return collection.GetAddress(courtType, court);
        }
        private static readonly List<AddressListDto> collection = AddressListDto.WilliamsonList;
    }
}