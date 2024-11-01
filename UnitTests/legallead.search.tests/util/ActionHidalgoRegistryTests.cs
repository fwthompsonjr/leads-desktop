using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using StructureMap;
using System;
using Thompson.RecordSearch.Utility.Interfaces;

namespace legallead.search.tests.util
{
    public class ActionHidalgoRegistryTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(ActionHidalgoRegistry));
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
        [InlineData("begin-search")]
        [InlineData("set-parameters")]
        [InlineData("check-no-count")]
        [InlineData("get-case-list")]
        public void ServiceCanGetNamedInstance(string serviceKey)
        {
            var service = GetContainer();
            var error = Record.Exception(() => _ = service.GetInstance<ICountySearchAction>(serviceKey));
            Assert.Null(error);
        }
        private static Container GetContainer()
        {
            return ActionHidalgoContainer.GetContainer;
        }
    }
}