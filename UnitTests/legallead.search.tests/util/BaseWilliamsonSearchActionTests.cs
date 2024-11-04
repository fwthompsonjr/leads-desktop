using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class BaseWilliamsonSearchActionTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(BaseWilliamsonSearchAction));
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
        public void ServiceHasWaitIntervalProperty()
        {
            var service = new MockSearchTesting();
            var error = Record.Exception(() => { _ = service.WaitInterval; });
            Assert.Null(error);
        }
        [Fact]
        public void ServiceHasPollingProperty()
        {
            var service = new MockSearchTesting();
            var error = Record.Exception(() => { _ = service.PollInterval; });
            Assert.Null(error);
        }
        [Fact]
        public void ServiceHasParametersProperty()
        {
            var service = new MockSearchTesting();
            var error = Record.Exception(() => { _ = service.Parameters; });
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

        [Fact]
        public void ServiceHasBaseJavaScriptExecutorMethod()
        {
            var service = new MockSearchTesting();
            var error = Record.Exception(() => { _ = service.GetBaseExecutor(); });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceHasNavigationMethod()
        {
            var service = new MockSearchTesting();
            var error = Record.Exception(() => { service.Navigation(); });
            Assert.NotNull(error);
            Assert.IsType<WebDriverTimeoutException>(error);
        }

        [Fact]
        public void ServiceHasJsScriptDefined()
        {
            var service = new MockSearchTesting();
            var actual = service.ViewJsScript();
            Assert.False(string.IsNullOrEmpty(actual));
        }
        [Fact]
        public void ServiceHasUriDefined()
        {
            var actual = MockSearchTesting.GetNavigationAddress();
            Assert.False(string.IsNullOrEmpty(actual));
        }

        private sealed class MockSearchTesting : BaseWilliamsonSearchAction
        {
            public MockSearchTesting()
            {
                var mq = new Mock<IWebDriver>();
                MqExecutor.Setup(s => s.ExecuteScript(It.IsAny<string>())).Returns("undefinded");
                Driver = mq.Object;
                Parameters = new LegalLead.PublicData.Search.Classes.DallasSearchProcess();
                _ = PageWaitPoolingInterval;
                _ = PageWaitTimeSpan;
            }
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                return MqExecutor.Object;
            }
            public void Navigation()
            {
                WaitForNavigation();
            }
            public static string GetNavigationAddress()
            {
                return NavigationUri();
            }
            public IJavaScriptExecutor GetBaseExecutor()
            {
                return base.GetJavaScriptExecutor();
            }

            public string ViewJsScript()
            {
                return JsScript;
            }
            public string GetScriptName => ScriptName;
            public TimeSpan WaitInterval => PageWaitTimeSpan;
            public TimeSpan PollInterval => PageWaitPoolingInterval;
            private Mock<IJavaScriptExecutor> MqExecutor { get; } = new Mock<IJavaScriptExecutor>();
            protected override TimeSpan PageWaitTimeSpan => TimeSpan.FromMilliseconds(100);
            protected override TimeSpan PageWaitPoolingInterval => TimeSpan.FromMilliseconds(25);
            protected override string ScriptName { get; } = "say hello";
        }
    }
}