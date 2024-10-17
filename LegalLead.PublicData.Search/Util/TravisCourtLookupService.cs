using System;
using System.Collections.Generic;
using System.Linq;
using Thompson.RecordSearch.Utility.Db;

namespace LegalLead.PublicData.Search.Util
{
    public static class TravisCourtLookupService
    {
        public static AddressListDto GetList(string name)
        {
            return collection.Find(x => x.Name.Equals(name, oic));
        }

        public static string GetAddress(string courtType, string court)
        {
            var list = GetList(courtType);
            if (list == null) return null;
            var fallback = GetFallbackAddress(list);
            var addr = list.Items.FirstOrDefault(x => x.Name.Equals(court, oic));
            if (addr == null) { return fallback; }
            return string.Join(" ", addr.Address).Trim();
        }

        private static string GetFallbackAddress(AddressListDto list)
        {
            var fallback = list.Items.FirstOrDefault();
            if (fallback == null) return null;
            var data = new List<string>();
            data.AddRange(fallback.Address);
            data.RemoveAt(0);
            return string.Join(" ", data).Trim();
        }

        private const StringComparison oic = StringComparison.OrdinalIgnoreCase;
        private static readonly List<AddressListDto> collection = AddressListDto.TravisList;
    }
}