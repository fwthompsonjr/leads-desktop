using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class TravisFetchAlternateCaseStyleTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(TravisFetchAlternateCaseStyle));
            Assert.Null(error);
        }
    }
}