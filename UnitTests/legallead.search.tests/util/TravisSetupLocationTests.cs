using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class TravisSetupLocationTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(TravisSetupLocation));
            Assert.Null(error);
        }
    }
}