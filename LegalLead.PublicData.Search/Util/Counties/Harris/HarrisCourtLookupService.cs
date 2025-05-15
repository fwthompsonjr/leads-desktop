using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace LegalLead.PublicData.Search.Util
{
    public static class HarrisCourtLookupService
    {
        public static string GetAddress(string court)
        {
            var fallback = Addresses[0];
            var found = Addresses.Find(x => x.Name.Equals(court)) ?? fallback;
            return found.Address;
        }
        private static string CourtJs => courtJs ??= GetCourtJs();
        private static List<HarrisCivilAddressDto> Addresses => _addresses ??= GetCourtList();

        private static string courtJs;
        private static List<HarrisCivilAddressDto> _addresses;
        private static string GetCourtJs()
        {
            var json = Properties.Resources.harris_civil_address_list;
            return Encoding.UTF8.GetString(json);
        }
        private static List<HarrisCivilAddressDto> GetCourtList()
        {
            var content = CourtJs;
            var data = JsonConvert.DeserializeObject<List<HarrisCivilAddressDto>>(content);
            return data ?? [];
        }
        private sealed class HarrisCivilAddressDto
        {
            [JsonProperty("address")] public string Address { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("fullName")] public string FullName { get; set; }
        }
    }
}