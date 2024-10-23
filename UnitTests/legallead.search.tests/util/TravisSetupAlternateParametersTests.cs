using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class TravisSetupAlternateParametersTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(TravisSetupAlternateParameters));
            Assert.Null(error);
        }
    }
}