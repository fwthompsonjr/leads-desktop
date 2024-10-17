using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Thompson.RecordSearch.Utility.Db
{
    public class AddressListDto
    {
        public string Name { get; set; }
        public IEnumerable<AddressItemDto> Items { get; set; } = Array.Empty<AddressItemDto>();

        public static List<AddressListDto> DallasList
        {
            get
            {
                if (dallasList != null) return dallasList;
                var tmp = JsonConvert.DeserializeObject<List<AddressListDto>>(dallasContent);
                dallasList = tmp;
                return dallasList;
            }
        }

        public static List<AddressListDto> TravisList
        {
            get
            {
                if (travisList != null) return travisList;
                var tmp = JsonConvert.DeserializeObject<List<AddressListDto>>(travisContent);
                travisList = tmp;
                return travisList;
            }
        }

        private static List<AddressListDto> travisList = null;
        private static List<AddressListDto> dallasList = null;

        private static readonly string dallasContent = Properties.Resources.dallas_court_address;
        private static readonly string travisContent = Properties.Resources.travis_court_address;
    }
}
