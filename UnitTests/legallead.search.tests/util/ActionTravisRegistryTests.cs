using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using StructureMap;
using System;
using Thompson.RecordSearch.Utility.Interfaces;

namespace legallead.search.tests.util
{
    public class ActionTravisRegistryTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(ActionTravisRegistry));
            Assert.Null(error);
        }

        [Theory]
        [InlineData(typeof(IHttpService))]
        [InlineData(typeof(ICountyCodeService))]
        [InlineData(typeof(ICountyCodeReader))]
        public void ServiceCanGenerateInstance(Type serviceType)
        {
            var service = GetContainer();
            var error = Record.Exception(() => _ = service.GetInstance(serviceType));
            Assert.Null(error);
        }

        [Theory]
        [InlineData("begin")]
        [InlineData("setup-location")]
        [InlineData("setup-options")]
        [InlineData("setup-parameters")]
        [InlineData("navigate")]
        [InlineData("fetch-items")]
        [InlineData("fetch-style")]
        public void ServiceCanGetNamedInstance(string serviceKey)
        {
            var service = GetContainer();
            var error = Record.Exception(() => _ = service.GetInstance<ITravisSearchAction>(serviceKey));
            Assert.Null(error);
        }
        private static Container GetContainer()
        {
            return ActionTravisContainer.GetContainer;
        }
    }
}