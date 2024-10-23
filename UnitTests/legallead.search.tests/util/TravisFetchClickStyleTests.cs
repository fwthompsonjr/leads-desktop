using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class TravisFetchClickStyleTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(TravisFetchClickStyle));
            Assert.Null(error);
        }
    }
}