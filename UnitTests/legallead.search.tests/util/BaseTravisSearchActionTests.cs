using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class BaseTravisSearchActionTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(BaseTravisSearchAction));
            Assert.Null(error);
        }
    }
}