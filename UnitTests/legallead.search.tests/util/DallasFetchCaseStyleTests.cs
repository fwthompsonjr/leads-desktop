using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class DallasFetchCaseStyleTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 70;
            var service = new DallasFetchCaseStyle();
            Assert.Equal(index, service.OrderId);
        }
        [Fact]
        public void ComponentCanExecute()
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var options = new Mock<IOptions>();
            var timeouts = new Mock<ITimeouts>();
            var parameters = new DallasSearchProcess();
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            driver.Setup(x => x.Manage()).Returns(options.Object);
            options.Setup(x => x.Timeouts()).Returns(timeouts.Object);
            timeouts.Setup(x => x.PageLoad).Returns(TimeSpan.FromSeconds(1));
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockDallasFetchCaseStyle
            {
                Parameters = parameters,
                Driver = driver.Object,
                PageAddress = "http://www.google.com"
            };
            _ = service.Execute();
            service.MqExecutor.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.AtLeast(3));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("abdcefg")]
        public void ComponentNeedsValidUrl(string address)
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockDallasFetchCaseStyle
            {
                Parameters = parameters,
                Driver = driver.Object,
                PageAddress = address
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ComponentThrowingException(int target)
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockDallasFetchCaseStyle
            {
                Parameters = target != 1 ? parameters : null,
                Driver = target != 0 ? driver.Object : null,
                PageAddress = "http://www.google.com"
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        private sealed class MockDallasFetchCaseStyle : DallasFetchCaseStyle
        {
            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                const string request = "return document.readyState";
                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s => s.Equals(request))))
                    .Returns("no")
                    .Returns("no")
                    .Returns("complete");

                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s => !s.Equals(request))))
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);
                return MqExecutor.Object;
            }
        }
    }
}