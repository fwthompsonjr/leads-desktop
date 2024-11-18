using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using System;
using Thompson.RecordSearch.Utility.Interfaces;

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
        public void ServiceCanGetNamedInstance(string type)
        {
            var error = Record.Exception(() =>
            {
                var item = SessionPersistenceContainer.GetContainer;
                var actual = item.GetInstance<ISessionPersistance>(type);
                Assert.NotNull(actual);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(typeof(IHttpService))]
        [InlineData(typeof(SessionUsageCapPersistence))]
        [InlineData(typeof(SessionUsagePersistence))]
        [InlineData(typeof(SessionUsageReader))]
        public void ServiceCanGetTypedInstance(Type type)
        {
            var error = Record.Exception(() =>
            {
                var item = SessionPersistenceContainer.GetContainer;
                var actual = item.GetInstance(type);
                Assert.NotNull(actual);
            });
            Assert.Null(error);
        }
    }
}