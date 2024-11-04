using System;
using System.Collections.Generic;
using System.Linq;
using Thompson.RecordSearch.Utility.Db;

namespace LegalLead.PublicData.Search.Common
{
    public static class AddressLookupExtensions
    {
        public static string GetAddress(this List<AddressListDto> collection, string courtType, string court)
        {
            var list = collection.Find(x => x.Name.Equals(courtType, oic));
            if (list == null) return null;
            var fallback = GetFallbackAddress(list);
            var addr = list.Items.FirstOrDefault(x => x.Name.Equals(court, oic));
            addr ??= collection.LookupAddress(court);
            if (addr == null)
            {
                return fallback;
            }
            return string.Join(" ", addr.Address).Trim();
        }

        public static AddressItemDto LookupAddress(this List<AddressListDto> collection, string court)
        {
            if (string.IsNullOrEmpty(court)) return null;
            var courtType = court.GetCourtType();
            var list = collection.Find(x => x.Name.Equals(courtType, oic));
            if (list == null) return null;
            var courtId = court.GetNumeric();
            if (string.IsNullOrEmpty(courtId)) return null;
            var addr = list.Items.FirstOrDefault(x =>
            {
                if ((courtType.Equals("county") || courtType.Equals("justice")) && int.TryParse(courtId, out var cidx)) return x.Id.Equals(cidx);
                if (courtType.Equals("district") && int.TryParse(courtId, out var _)) return x.Name.StartsWith(courtId);
                return false;
            });
            return addr;
        }

        public static string GetCourtType(this string court)
        {
            if (string.IsNullOrWhiteSpace(court)) return string.Empty;
            var courtType = "county";
            if (court.Contains("justice", oic) || court.Contains("precinct", oic)) courtType = "justice";
            if (court.Contains("district", oic)) courtType = "district";
            return courtType;
        }
        public static string GetNumeric(this string source)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;
            var items = source.ToCharArray().ToList();
            var output = string.Empty;
            foreach (var item in items)
            {
                if (char.IsDigit(item)) output += item;
                if (!char.IsDigit(item) && !string.IsNullOrEmpty(output)) break;
            }
            return output;
        }

        private static string GetFallbackAddress(AddressListDto list)
        {
            var fallback = list.Items.FirstOrDefault();
            if (fallback == null) return string.Empty;
            var data = new List<string>();
            data.AddRange(fallback.Address);
            data.RemoveAt(0);
            return string.Join(" ", data).Trim();
        }

        private const StringComparison oic = StringComparison.OrdinalIgnoreCase;
    }
}
