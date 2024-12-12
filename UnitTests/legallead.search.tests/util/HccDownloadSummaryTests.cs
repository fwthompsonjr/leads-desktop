using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class HccDownloadSummaryTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 20;
            var service = new HccDownloadSummary();
            Assert.Equal(index, service.OrderId);
        }


        [Fact]
        public void ComponentHasBaseExecutor()
        {
            var service = new MockHccDownloadSummary();
            var executor = service.GetBaseExecutor();
            Assert.Null(executor);
        }

        [Theory]
        [InlineData("JUSTICE")]
        [InlineData("DISTRICT")]
        [InlineData("COUNTY")]
        public void ComponentCanExecute(string locator)
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            var element = new Mock<IWebElement>();
            var startDt = DateTime.Now;
            var endingDt = DateTime.Now.AddDays(3);
            parameters.SetSearchParameters(startDt, endingDt, locator);
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            driver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(element.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockHccDownloadSummary
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            _ = service.Execute();
            service.MqExecutor.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.AtLeast(1));
        }

        private sealed class MockHccDownloadSummary : HccDownloadSummary
        {
            public MockHccDownloadSummary()
            {
                IsDownloadRequested = true;
                IsTestMode = true;
            }
            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                const string request = "return document.readyState";
                const string response = "complete";
                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s => !s.Equals(request))))
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);

                MqExecutor.Setup(x => x.ExecuteScript(It.Is<string>(s => s.Equals(request))))
                    .Returns(response);
                return MqExecutor.Object;
            }
            public IJavaScriptExecutor GetBaseExecutor()
            {
                return base.GetJavaScriptExecutor();
            }
        }
    }
}