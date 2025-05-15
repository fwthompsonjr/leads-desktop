using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Db;

namespace LegalLead.PublicData.Search.Util
{
    public static class HccCourtLookupService
    {
        public static string GetAddress(string court)
        {
            var fallback = FallbackAddress;
            var item = collection.Find(x => x.Code.Equals(court));
            if (item == null) return fallback;
            var data = new[] { item.Name, item.Address };
            return string.Join(Environment.NewLine, data);
        }

        private static string FallbackAddress
        {
            get
            {
                var data = new[] { "Harris County Criminal Justice Center", "1201 Franklin St, Houston Texas 77002" };
                return string.Join(Environment.NewLine, data);
            }
        }
        private static readonly List<HccAddressItem> collection = HccAddressItem.HccList;
    }
}