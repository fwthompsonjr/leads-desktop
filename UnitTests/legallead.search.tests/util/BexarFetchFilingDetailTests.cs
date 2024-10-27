using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;

namespace legallead.search.tests.util
{
    public class BexarFetchFilingDetailTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(BexarFetchFilingDetail));
            Assert.Null(error);
        }
        [Fact]
        public void ServiceCanBeCreated()
        {
            var error = Record.Exception(() => _ = new MockBexarFetchFilingDetail());
            Assert.Null(error);
        }

        [Fact]
        public void ServiceHasExpectedOrderId()
        {
            var error = Record.Exception(() =>
            {
                var service = new MockBexarFetchFilingDetail();
                var actual = service.OrderId;
                Assert.Equal(70, actual);
            });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceWithoutDriverWillNotExecute()
        {
            var service = new BexarFetchFilingDetail();
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ServiceCanBeExecuted(bool hasItems)
        {
            var error = Record.Exception(() =>
            {
                var service = new MockBexarFetchFilingDetail(hasItems);
                _ = service.Execute();
            });
            Assert.Null(error);
        }
        private sealed class MockBexarFetchFilingDetail : BexarFetchFilingDetail
        {
            public MockBexarFetchFilingDetail(bool withItems = true)
            {
                const string uri = "http://localhost/testing";
                var collection = withItems ? new[] { MqElement.Object } : Array.Empty<IWebElement>();
                var roCollection = new ReadOnlyCollection<IWebElement>(collection);
                MqDriver.Setup(x => x.Navigate()).Returns(MqNavigation.Object);
                MqDriver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(MqElement.Object);
                MqDriver.Setup(x => x.FindElements(It.IsAny<By>())).Returns(roCollection);
                MqDriver.SetupGet(x => x.Url).Returns(uri);
                MqNavigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
                MqElement.Setup(x => x.GetAttribute(It.Is<string>(s => s == "data-url"))).Returns($"{uri}/RegisterOfActions/?id");
                MqElement.Setup(x => x.GetAttribute(It.Is<string>(s => s == "style"))).Returns("color: blue");
                MqElement.Setup(x => x.GetAttribute(It.Is<string>(s => s == "innerHTML"))).Returns(Properties.Resources.bexar_single_page);
                Driver = MqDriver.Object;
            }
            public Mock<IWebDriver> MqDriver { get; private set; } = new Mock<IWebDriver>();
            public Mock<INavigation> MqNavigation { get; private set; } = new Mock<INavigation>();
            public Mock<IWebElement> MqElement { get; private set; } = new Mock<IWebElement>();
            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                MqExecutor.Setup(x => x.ExecuteScript(It.IsAny<string>()))
                    .Returns(string.Empty);
                return MqExecutor.Object;
            }
        }
    }
}