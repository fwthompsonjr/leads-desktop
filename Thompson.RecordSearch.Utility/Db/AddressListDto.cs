using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Thompson.RecordSearch.Utility.Db
{
    public class AddressListDto
    {
        public string Name { get; set; }
        public IEnumerable<AddressItemDto> Items { get; set; } = Array.Empty<AddressItemDto>();

        public static List<AddressListDto> BexarList
        {
            get
            {
                if (bexarList != null) return bexarList;
                var tmp = JsonConvert.DeserializeObject<List<AddressListDto>>(bexarContent);
                bexarList = tmp;
                return bexarList;
            }
        }

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
        public static List<AddressListDto> HidalgoList
        {
            get
            {
                if (hidalgoList != null) return hidalgoList;
                var tmp = JsonConvert.DeserializeObject<List<AddressListDto>>(hidalgoContent);
                hidalgoList = tmp;
                return hidalgoList;
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

        public static List<AddressListDto> ElPasoList
        {
            get
            {
                if (elpasoList != null) return elpasoList;
                var tmp = JsonConvert.DeserializeObject<List<AddressListDto>>(elpasoContent);
                elpasoList = tmp;
                return elpasoList;
            }
        }

        public static List<AddressListDto> FortBendList
        {
            get
            {
                if (fortbendList != null) return fortbendList;
                var tmp = JsonConvert.DeserializeObject<List<AddressListDto>>(fortbendContent);
                fortbendList = tmp;
                return fortbendList;
            }
        }

        public static List<AddressListDto> WilliamsonList
        {
            get
            {
                if (williamsonList != null) return williamsonList;
                var tmp = JsonConvert.DeserializeObject<List<AddressListDto>>(williamsonContent);
                williamsonList = tmp;
                return williamsonList;
            }
        }

        private static List<AddressListDto> bexarList = null;
        private static List<AddressListDto> dallasList = null;
        private static List<AddressListDto> travisList = null;
        private static List<AddressListDto> hidalgoList = null;
        private static List<AddressListDto> elpasoList = null;
        private static List<AddressListDto> fortbendList = null;
        private static List<AddressListDto> williamsonList = null;

        private static readonly string bexarContent = Properties.Resources.bexar_court_address;
        private static readonly string dallasContent = Properties.Resources.dallas_court_address;
        private static readonly string elpasoContent = Properties.Resources.elpaso_court_address;
        private static readonly string travisContent = Properties.Resources.travis_court_address;
        private static readonly string hidalgoContent = Properties.Resources.hidalgo_court_address;
        private static readonly string fortbendContent = Properties.Resources.fortbend_court_address;
        private static readonly string williamsonContent = Properties.Resources.williamson_court_address;
    }
}
