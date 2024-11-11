using LegalLead.PublicData.Search.Helpers;
using System;
using Thompson.RecordSearch.Utility.Interfaces;

namespace legallead.search.tests.helpers
{
    public class AuthenicationContainerTests
    {
        [Fact]
        public void InstanceContainsContainer()
        {
            var error = Record.Exception(() =>
            {
                var item = AuthenicationContainer.GetContainer;
                Assert.NotNull(item);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(typeof(IHttpService))]
        [InlineData(typeof(IAuthenicationService))]
        public void ServiceCanGetTypedInstance(Type type)
        {
            var error = Record.Exception(() =>
            {
                var item = AuthenicationContainer.GetContainer;
                var actual = item.GetInstance(type);
                Assert.NotNull(actual);
            });
            Assert.Null(error);
        }
    }
}