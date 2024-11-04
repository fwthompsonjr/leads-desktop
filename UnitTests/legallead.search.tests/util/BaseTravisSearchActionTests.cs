using LegalLead.PublicData.Search.Util;

namespace legallead.search.tests.util
{
    public class BaseTravisSearchActionTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(BaseTravisSearchAction));
            Assert.Null(error);
        }
        [Fact]
        public void ServiceHasOrderIdProperty()
        {
            var service = new MockSearchTesting();
            Assert.Equal(0, service.OrderId);
        }
        [Fact]
        public void ServiceHasScriptNameProperty()
        {
            var service = new MockSearchTesting();
            var error = Record.Exception(() => { _ = service.GetScriptName; });
            Assert.Null(error);
        }
        [Fact]
        public void ServiceHasIsPostSearchProperty()
        {
            var service = new MockSearchTesting();
            var error = Record.Exception(() => { _ = service.IsPostSearch; });
            Assert.Null(error);
        }
        [Fact]
        public void ServiceHasExecuteMethod()
        {
            var service = new MockSearchTesting();
            var error = Record.Exception(() => { _ = service.Execute(); });
            Assert.Null(error);
        }
        [Fact]
        public void ServiceHasJavaScriptExecutorMethod()
        {
            var service = new MockSearchTesting();
            var error = Record.Exception(() => { _ = service.GetJavaScriptExecutor(); });
            Assert.Null(error);
        }
        private sealed class MockSearchTesting : BaseTravisSearchAction
        {
            public string GetScriptName => ScriptName;
        }
    }
}