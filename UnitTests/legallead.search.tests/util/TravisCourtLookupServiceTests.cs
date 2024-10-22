using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class TravisCourtLookupServiceTests
    {
        [Theory]
        [InlineData("Justice", "")]
        [InlineData("Justice", "Precinct 1")]
        [InlineData("Justice", "Precinct 2")]
        [InlineData("Justice", "Precinct 3")]
        [InlineData("Justice", "Precinct 4")]
        [InlineData("Justice", "Precinct 5")]
        public void ServiceCanGetAddress(string courtType, string court)
        {
            var address = TravisCourtLookupService.GetAddress(courtType, court);
            Assert.False(string.IsNullOrEmpty(address));
        }
    }
}
