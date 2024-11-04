using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class WilliamsonNavigateCourtPageTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 15;
            var service = new WilliamsonNavigateCourtPage();
            Assert.Equal(index, service.OrderId);
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
            var startDt = DateTime.Now;
            var endingDt = DateTime.Now.AddDays(3);
            parameters.Search(startDt, endingDt, locator);
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockWilliamsonNavigateCourtPage
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            _ = service.Execute();
            service.MqExecutor.Verify(x => x.ExecuteScript(It.IsAny<string>()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ComponentThrowingException(int target, string searchType = "JUSTICE", bool hasStartDate = true, bool hasEndingDate = true)
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            DateTime? startDt = hasStartDate ? DateTime.Now : null;
            DateTime? endingDt = hasEndingDate ? DateTime.Now.AddDays(3) : null;
            parameters.Search(startDt, endingDt, searchType);
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockWilliamsonNavigateCourtPage
            {
                Parameters = target != 1 ? parameters : null,
                Driver = target != 0 ? driver.Object : null
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        private sealed class MockWilliamsonNavigateCourtPage : WilliamsonNavigateCourtPage
        {
            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                const string request = "return document.readyState";
                const string response = "complete";
                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s => !s.Equals(request))))
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);

                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s => s.Equals(request))))
                    .Returns(0)
                    .Returns(string.Empty)
                    .Returns(response);
                return MqExecutor.Object;
            }
        }
    }
}