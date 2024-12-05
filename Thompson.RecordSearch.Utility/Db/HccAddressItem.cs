using Newtonsoft.Json;
using System.Collections.Generic;

namespace Thompson.RecordSearch.Utility.Db
{
    public class HccAddressItem
    {
        public int Id { get; set; } = -1;
        public string Code { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Name
        {
            get
            {
                if (Id < 0) return string.Empty;
                if (Id < 17) return $"Harris County Criminal Court at Law No. {Id}";
                return $"Harris County {Id}{GetOridinal(Id)} Criminal Court at Law";
            }
        }
        private static string GetOridinal(int number)
        {
            var str = number.ToString("d").ToCharArray();
            var ln = str.Length - 1;
            var result = "th";
            var ch = str[ln];
            if (ch == '1') { result = "st"; }
            if (ch == '2') { result = "nd"; }
            if (ch == '3') { result = "rd"; }
            return result;
        }

        public static List<HccAddressItem> HccList
        {
            get
            {
                if (hccList != null) return hccList;
                var tmp = JsonConvert.DeserializeObject<List<HccAddressItem>>(hccContent);
                hccList = tmp;
                return hccList;
            }
        }
        private static List<HccAddressItem> hccList = null;
        private static readonly string hccContent = Properties.Resources.hcc_court_list;
    }
}
