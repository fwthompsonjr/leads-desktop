using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class TravisNavigateAlternateSearchTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(TravisNavigateAlternateSearch));
            Assert.Null(error);
        }
    }
}