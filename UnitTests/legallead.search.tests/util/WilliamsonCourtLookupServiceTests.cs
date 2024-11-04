using LegalLead.PublicData.Search.Util;
using System.Linq;
using Thompson.RecordSearch.Utility.Db;

namespace legallead.search.tests.util
{
    public class WilliamsonCourtLookupServiceTests
    {
        [Theory]
        [InlineData("Justice", "")]
        [InlineData("Justice", "Precinct 1")]
        [InlineData("Justice", "Precinct 2")]
        [InlineData("Justice", "Precinct 3")]
        [InlineData("Justice", "Precinct 4")]
        [InlineData("County", "")]
        [InlineData("County", "Court 1")]
        [InlineData("County", "Court 2")]
        [InlineData("County", "Court 3")]
        [InlineData("County", "Court 4")]
        [InlineData("County", "Court 5")]
        [InlineData("District", "")]
        [InlineData("District", "26th Civil District 1")]
        [InlineData("District", "Civil 277District 2")]
        [InlineData("District", "368th Civil District 3")]
        [InlineData("District", "Civil 395 District 4")]
        [InlineData("District", "425 Civil District 5")]
        [InlineData("District", "480th Civil District 7")]
        [InlineData("District", "Civil District 448 11")]
        public void ServiceCanGetAddress(string courtType, string court)
        {
            var address = WilliamsonCourtLookupService.GetAddress(courtType, court);
            Assert.False(string.IsNullOrEmpty(address));
        }
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(4, true)]
        [InlineData(5, true)]
        [InlineData(6, false)]
        [InlineData(7, false)]
        [InlineData(8, false)]
        [InlineData(9, false)]
        public void ServiceCanMapCounty(int index, bool expected)
        {
            var counties = AddressListDto.WilliamsonList.Find(x => x.Name == "county");
            Assert.NotNull(counties);
            var item = counties.Items.FirstOrDefault(x => x.Id == index);
            if (!expected && item != null)
            {
                Assert.Fail($"Unexpexted index {index} was found in collection");
            }
            if (!expected) return;
            Assert.NotNull(item);
            var find = $"{index}";
            Assert.EndsWith(find, item.Name, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}