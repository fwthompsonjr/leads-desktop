using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;

namespace legallead.search.tests.helpers
{
    public class SessionPersistenceContainerTests
    {
        [Fact]
        public void InstanceContainsContainer()
        {
            var error = Record.Exception(() =>
            {
                var item = SessionPersistenceContainer.GetContainer;
                Assert.NotNull(item);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("legacy")]
        [InlineData("enhanced")]
        public void ServiceCanGetTypedInstance(string type)
        {
            var error = Record.Exception(() =>
            {
                var item = SessionPersistenceContainer.GetContainer;
                var actual = item.GetInstance<ISessionPersistance>(type);
                Assert.NotNull(actual);
            });
            Assert.Null(error);
        }
    }
}