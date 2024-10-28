using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class BexarCourtLookupServiceTests
    {
        [Theory]
        [InlineData("Justice", "")]
        [InlineData("Justice", "Precinct 1")]
        [InlineData("Justice", "Precinct 2")]
        [InlineData("Justice", "Precinct 3")]
        [InlineData("Justice", "Precinct 4")]
        [InlineData("Justice", "Precinct 0")]
        [InlineData("Justice", "Prec. 1")]
        [InlineData("Justice", "Prec. 2")]
        [InlineData("Justice", "Prec. 3")]
        [InlineData("Justice", "Precinct, 4")]
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
        [InlineData("County", "Court 11")]
        [InlineData("County", "Court 12")]
        [InlineData("County", "Court 13")]
        [InlineData("County", "Court 14")]
        [InlineData("County", "Court 15")]
        [InlineData("District", "")]
        [InlineData("District", "37th Civil District 1")]
        [InlineData("District", "Civil 45 District 2")]
        [InlineData("District", "57th Civil District 3")]
        [InlineData("District", "Civil 73 District 4")]
        [InlineData("District", "131 Civil District 5")]
        [InlineData("District", "Civil 150th District 6")]
        [InlineData("District", "166th Civil District 7")]
        [InlineData("District", "Civil 224th District 8")]
        [InlineData("District", "Civil 285th District 9")]
        [InlineData("District", "288Civil District 10")]
        [InlineData("District", "Civil District 407 11")]
        [InlineData("District", "Civil District 408 12")]
        [InlineData("District", "438Civil District 13")]
        [InlineData("District", "Civil District 14")]
        public void ServiceCanGetAddress(string courtType, string court)
        {
            var address = BexarCourtLookupService.GetAddress(courtType, court);
            Assert.False(string.IsNullOrEmpty(address));
        }
    }
}