using System;
using System.Collections.Generic;
using System.Linq;
using Thompson.RecordSearch.Utility.Db;

namespace LegalLead.PublicData.Search.Util
{
    public static class BexarCourtLookupService
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
            addr ??= LookupAddress(court);
            if (addr == null)
            {
                return fallback;
            }
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
        private static AddressItemDto LookupAddress(string court)
        {
            if (string.IsNullOrEmpty(court)) return null;
            var courtType = "county";
            if (court.Contains("justice", oic) || court.Contains("precinct", oic)) courtType = "justice";
            if (court.Contains("district", oic)) courtType = "district";
            var list = GetList(courtType);
            if (list == null) return null;
            var courtId = GetNumeric(court);
            if (string.IsNullOrEmpty(courtId)) return null;
            var addr = list.Items.FirstOrDefault(x =>
            {
                if ((courtType.Equals("county") || courtType.Equals("justice")) && int.TryParse(courtId, out var cidx)) return x.Id.Equals(cidx);
                if (courtType.Equals("district") && int.TryParse(courtId, out var _)) return x.Name.StartsWith(courtId);
                return false;
            });
            return addr;
        }

        private static string GetNumeric(string source)
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
        private const StringComparison oic = StringComparison.OrdinalIgnoreCase;
        private static readonly List<AddressListDto> collection = AddressListDto.BexarList;
    }
}