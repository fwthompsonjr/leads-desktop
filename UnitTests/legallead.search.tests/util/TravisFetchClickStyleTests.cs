using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;

namespace legallead.search.tests.util
{
    public class TravisFetchClickStyleTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(TravisFetchClickStyle));
            Assert.Null(error);
        }
        [Fact]
        public void ServiceCanBeCreated()
        {
            var error = Record.Exception(() => _ = new MockTravisFetchClickStyle());
            Assert.Null(error);
        }

        [Fact]
        public void ServiceCanBeExecuted()
        {
            var error = Record.Exception(() =>
            {
                var service = new MockTravisFetchClickStyle();
                _ = service.Execute();
            });
            Assert.Null(error);
        }
        private sealed class MockTravisFetchClickStyle : TravisFetchClickStyle
        {
            public MockTravisFetchClickStyle()
            {
                const string uri = "http://localhost/testing";
                var collection = new[] { MqElement.Object };
                var roCollection = new ReadOnlyCollection<IWebElement>(collection);
                MqDriver.Setup(x => x.Navigate()).Returns(MqNavigation.Object);
                MqDriver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(MqElement.Object);
                MqDriver.Setup(x => x.FindElements(It.IsAny<By>())).Returns(roCollection);
                MqDriver.SetupGet(x => x.Url).Returns(uri);
                MqNavigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
                MqElement.Setup(x => x.GetAttribute(It.Is<string>(s => s == "href"))).Returns($"{uri}?CaseDetail.aspx?id=112321");
                MqElement.Setup(x => x.GetAttribute(It.Is<string>(s => s == "style"))).Returns("color: blue");
                MqElement.Setup(x => x.GetAttribute(It.Is<string>(s => s == "innerHTML"))).Returns(Properties.Resources.travis_single_page);
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