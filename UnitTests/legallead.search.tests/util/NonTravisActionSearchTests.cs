using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class NonTravisActionSearchTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(NonTravisActionSearch));
            Assert.Null(error);
        }
    }
}