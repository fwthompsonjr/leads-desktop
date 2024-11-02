using LegalLead.PublicData.Search.Util;
using System.Linq;
using Thompson.RecordSearch.Utility.Db;

namespace legallead.search.tests.util
{
    public class HidalgoCourtLookupServiceTests
    {
        [Theory]
        [InlineData("Justice", "")]
        [InlineData("Justice", "Precinct 1")]
        [InlineData("Justice", "Precinct 2")]
        [InlineData("Justice", "Precinct 3")]
        [InlineData("Justice", "Precinct 4")]
        [InlineData("Justice", "Precinct 5")]
        [InlineData("Justice", "Precinct 0")]
        [InlineData("Justice", "Prec. 1")]
        [InlineData("Justice", "Prec. 2")]
        [InlineData("Justice", "Prec. 3")]
        [InlineData("Justice", "Precinct, 4")]
        [InlineData("Justice", "Precinct 1, Place 1")]
        [InlineData("Justice", "Precinct 1, Place 2")]
        [InlineData("Justice", "Precinct 2, Place 1")]
        [InlineData("Justice", "Precinct 2, Place 2")]
        [InlineData("Justice", "Precinct 3, Place 1")]
        [InlineData("Justice", "Precinct 3, Place 2")]
        [InlineData("Justice", "Precinct 4, Place 1")]
        [InlineData("Justice", "Precinct 4, Place 2")]
        [InlineData("Justice", "Precinct 5, Place 1")]
        [InlineData("County", "")]
        [InlineData("County", "Court 1")]
        [InlineData("County", "Court 2")]
        [InlineData("County", "Court 3")]
        [InlineData("County", "Court 4")]
        [InlineData("County", "Court 5")]
        [InlineData("County", "Court 6")]
        [InlineData("County", "Court 7")]
        [InlineData("County", "Court 8")]
        [InlineData("County", "Court 9")]
        [InlineData("County", "Court 10")]
        [InlineData("District", "")]
        [InlineData("District", "92th Civil District 1")]
        [InlineData("District", "Civil 93 District 2")]
        [InlineData("District", "139th Civil District 3")]
        [InlineData("District", "Civil 206 District 4")]
        [InlineData("District", "275 Civil District 5")]
        [InlineData("District", "Civil 332th District 6")]
        [InlineData("District", "370th Civil District 7")]
        [InlineData("District", "Civil 389th District 8")]
        [InlineData("District", "Civil 398th District 9")]
        [InlineData("District", "430Civil District 10")]
        [InlineData("District", "Civil District 464 11")]
        [InlineData("District", "Civil District 449 12")]
        [InlineData("District", "476Civil District 13")]
        [InlineData("District", "Civil District 14")]
        public void ServiceCanGetAddress(string courtType, string court)
        {
            var address = HidalgoCourtLookupService.GetAddress(courtType, court);
            Assert.False(string.IsNullOrEmpty(address));
        }
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        [InlineData(4, true)]
        [InlineData(5, true)]
        [InlineData(6, true)]
        [InlineData(7, true)]
        [InlineData(8, true)]
        [InlineData(9, true)]
        [InlineData(10, true)]
        public void ServiceCanMapCounty(int index, bool expected)
        {
            var counties = AddressListDto.HidalgoList.Find(x => x.Name == "county");
            Assert.NotNull(counties);
            var item = counties.Items.FirstOrDefault(x => x.Id == index);
            if (!expected && item != null)
            {
                Assert.Fail($"Unexpexted index {index} was found in collection");
            }
            if (!expected) return;
            Assert.NotNull(item);
            var find = $"#{index}";
            Assert.EndsWith(find, item.Name, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}