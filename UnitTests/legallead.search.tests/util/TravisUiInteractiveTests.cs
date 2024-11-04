using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class TravisUiInteractiveTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(TravisUiInteractive));
            Assert.Null(error);
        }
    }
}