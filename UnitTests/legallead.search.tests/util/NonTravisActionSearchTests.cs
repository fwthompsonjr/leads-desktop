using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using StructureMap;

namespace legallead.search.tests.util
{
    public class NonTravisActionSearchTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(NonTravisActionSearch));
            Assert.Null(error);
        }
        [Fact]
        public void ServiceCanBeInitiated()
        {
            var service = GetService();
            Assert.NotNull(service);
        }

        [Fact]
        public void ServiceHasOrderIdProperty()
        {
            var service = GetService();
            Assert.Equal(-1, service.OrderId);
        }

        [Fact]
        public void ServiceHasDriverProperty()
        {
            var service = GetService();
            var error = Record.Exception(() => { _ = service.Driver; });
            Assert.Null(error);
        }
        [Fact]
        public void ServiceHasParametersProperty()
        {
            var service = GetService();
            var error = Record.Exception(() => { _ = service.Parameters; });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceHasIsPostSearchProperty()
        {
            var service = GetService();
            var error = Record.Exception(() => { _ = service.IsPostSearch; });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceHasExecuteMethod()
        {
            var service = GetService();
            var error = Record.Exception(() => { _ = service.Execute(); });
            Assert.Null(error);
        }
        private static NonTravisActionSearch GetService()
        {
            var service = container.GetInstance<ITravisSearchAction>();
            if (service is NonTravisActionSearch action) return action;
            return default;
        }
        private static readonly Container container = new(new ActionZeroRegistry());
    }
}