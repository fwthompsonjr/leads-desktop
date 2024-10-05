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

        private static List<AddressListDto> dallasList = null;
        private static readonly string dallasContent = Properties.Resources.dallas_court_address;
    }
}
