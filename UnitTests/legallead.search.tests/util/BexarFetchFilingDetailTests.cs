using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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
                var list1 = windowIndexes.Split(',').ToList();
                var list2 = list1.FindAll(x => x == list1[0]);
                var collection = withItems ? new[] { MqElement.Object } : Array.Empty<IWebElement>();
                var roCollection = new ReadOnlyCollection<IWebElement>(collection);
                var handles1 = new ReadOnlyCollection<string>(list1);
                var handles2 = new ReadOnlyCollection<string>(list2);
                MqDriver.SetupSequence(s => s.WindowHandles)
                    .Returns(handles1)
                    .Returns(handles1)
                    .Returns(handles2)
                    .Returns(handles2)
                    .Returns(handles2)
                    .Returns(handles2);

                MqDriver.Setup(x => x.Navigate()).Returns(MqNavigation.Object);
                MqDriver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(MqElement.Object);
                MqDriver.Setup(x => x.FindElements(It.IsAny<By>())).Returns(roCollection);
                MqDriver.Setup(x => x.SwitchTo()).Returns(MqTarget.Object);
                MqDriver.SetupGet(x => x.Url).Returns(uri);
                MqNavigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
                MqElement.Setup(x => x.GetAttribute(It.Is<string>(s => s == "data-url"))).Returns($"{uri}/RegisterOfActions/?id");
                MqElement.Setup(x => x.GetAttribute(It.Is<string>(s => s == "style"))).Returns("color: blue");
                MqElement.Setup(x => x.GetAttribute(It.Is<string>(s => s == "innerHTML"))).Returns(Properties.Resources.bexar_single_page);
                MqTarget.Setup(m => m.Window(It.IsAny<string>())).Returns(MqDriver.Object);
                Driver = MqDriver.Object;
            }
            public Mock<IWebDriver> MqDriver { get; private set; } = new Mock<IWebDriver>();
            public Mock<INavigation> MqNavigation { get; private set; } = new Mock<INavigation>();
            public Mock<IWebElement> MqElement { get; private set; } = new Mock<IWebElement>();
            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public Mock<ITargetLocator> MqTarget { get; private set; } = new Mock<ITargetLocator>();
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                MqExecutor.Setup(x => x.ExecuteScript(It.IsAny<string>()))
                    .Returns(string.Empty);
                return MqExecutor.Object;
            }

            private static string windowIndexes = "0,1";
        }
    }
}