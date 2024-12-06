using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class HccCourtLookupServiceTests
    {
        [Theory]
        [InlineData("001")]
        [InlineData("002")]
        [InlineData("003")]
        [InlineData("004")]
        [InlineData("005")]
        [InlineData("006")]
        [InlineData("007")]
        [InlineData("008")]
        [InlineData("009")]
        [InlineData("010")]
        [InlineData("011")]
        [InlineData("012")]
        [InlineData("013")]
        [InlineData("014")]
        [InlineData("015")]
        [InlineData("016")]
        [InlineData("na")]
        public void ServiceCanFindAddress(string code)
        {
            var lookup = HccCourtLookupService.GetAddress(code);
            var actual = string.IsNullOrEmpty(lookup);
            Assert.False(actual);
        }
    }
}
