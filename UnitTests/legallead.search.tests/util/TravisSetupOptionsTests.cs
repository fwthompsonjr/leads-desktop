using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class TravisSetupOptionsTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(TravisSetupOptions));
            Assert.Null(error);
        }
    }
}