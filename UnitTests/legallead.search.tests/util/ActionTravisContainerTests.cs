using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class ActionTravisContainerTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(ActionTravisContainer));
            Assert.Null(error);
        }
    }
}