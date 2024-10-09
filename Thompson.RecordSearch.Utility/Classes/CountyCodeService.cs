using Newtonsoft.Json;
using System;
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

        private static readonly string mapJs = Properties.Resources.credential_service_map;
    }
}
