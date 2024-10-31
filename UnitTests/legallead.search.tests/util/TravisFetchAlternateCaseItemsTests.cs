using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class TravisFetchAlternateCaseItemsTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(TravisFetchAlternateCaseItems));
            Assert.Null(error);
        }
    }
}