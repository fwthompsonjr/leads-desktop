using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Diagnostics;
using System.Linq;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Interfaces;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class CountyCodeService : ICountyCodeService
    {
        public CountyCodeMapDto Map => GetMap;

        public CountyCodeDto Find(int id)
        {
            if (Map == null) return null;
            if (!Map.Counties.Any()) return null;
            var list = Map.Counties.ToList();
            return list.Find(x => x.Id == id);
        }

        public CountyCodeDto Find(string name)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            if (Map == null) return null;
            if (!Map.Counties.Any()) return null;
            var list = Map.Counties.ToList();
            return list.Find(x => x.Name.Equals(name, oic));
        }

        public string GetWebAddress(int id)
        {
            if (!indexes.Contains(id)) return string.Empty;
            if (Map == null || string.IsNullOrEmpty(Map.Web)) return string.Empty;
            var suffix = GetSuffix(id);
            if (string.IsNullOrEmpty(suffix)) return string.Empty;
            return string.Concat(Map.Web, suffix);
        }

        private string GetSuffix(int id)
        {
            if (!indexes.Contains(id)) return string.Empty;
            var dto = id < 10 ? Map.Landings : Map.ApiLandings;
            var pos = id % 10;
            switch (pos)
            {
                case 0: return dto.Login ?? string.Empty;
                case 1: return dto.County ?? string.Empty;
                case 2: return dto.Change ?? string.Empty;
                case 3: return dto.Indexes ?? string.Empty;
                case 4: return dto.Register ?? string.Empty;
                default: return string.Empty;
            }
        }
        private static readonly int[] indexes = new[] { 0, 1, 10, 11, 12, 13, 14 };
        private static CountyCodeMapDto GetMap
        {
            get
            {
                if (map != null) return map;
                map = JsonConvert.DeserializeObject<CountyCodeMapDto>(mapJs) ?? new CountyCodeMapDto();
                return map;
            }
        }

        private static CountyCodeMapDto map = null;

        private static readonly string mapJs = Properties.Resources.credential_services_map;
    }
}
