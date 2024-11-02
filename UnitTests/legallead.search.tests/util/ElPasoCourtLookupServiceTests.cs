using LegalLead.PublicData.Search.Util;
using System.Linq;
using Thompson.RecordSearch.Utility.Db;

namespace legallead.search.tests.util
{
    public class ElPasoCourtLookupServiceTests
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
        [InlineData("County", "Court 3")]
        [InlineData("County", "Court 6")]
        [InlineData("County", "Court 7")]
        [InlineData("County", "Court 8")]
        [InlineData("County", "Court 9")]
        [InlineData("District", "")]
        [InlineData("District", "34th Civil District 1")]
        [InlineData("District", "Civil 168 District 2")]
        [InlineData("District", "243th Civil District 3")]
        [InlineData("District", "Civil 171 District 4")]
        [InlineData("District", "120 Civil District 5")]
        [InlineData("District", "Civil 210th District 6")]
        [InlineData("District", "346th Civil District 7")]
        [InlineData("District", "Civil 384th District 8")]
        [InlineData("District", "Civil 41st District 9")]
        [InlineData("District", "205thCivil District 10")]
        [InlineData("District", "Civil District 448 11")]
        public void ServiceCanGetAddress(string courtType, string court)
        {
            var address = ElPasoCourtLookupService.GetAddress(courtType, court);
            Assert.False(string.IsNullOrEmpty(address));
        }
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, true)]
        [InlineData(4, false)]
        [InlineData(5, false)]
        [InlineData(6, true)]
        [InlineData(7, true)]
        [InlineData(8, false)]
        [InlineData(9, false)]
        public void ServiceCanMapCounty(int index, bool expected)
        {
            var counties = AddressListDto.ElPasoList.Find(x => x.Name == "county");
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