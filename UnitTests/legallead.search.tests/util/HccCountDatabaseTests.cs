using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class HccCountDatabaseTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 2;
            var service = new HccCountDatabase();
            Assert.Equal(index, service.OrderId);
        }
        [Fact]
        public void ComponentHasHccServiceDefined()
        {
            var service = new HccCountDatabase();
            Assert.Null(service.HccService);
        }

        [Fact]
        public void ComponentReturnsZeroWhenServiceIsNull()
        {
            var service = new HccCountDatabase();
            var actual = service.Execute();
            if (actual is int expected) Assert.Equal(0, expected);
        }
        [Fact]
        public void ComponentCanExecute()
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            var element = new Mock<IWebElement>();
            var startDt = DateTime.Now;
            var endingDt = DateTime.Now.AddDays(3);
            parameters.SetSearchParameters(startDt, endingDt, "CRIMINAL");
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            driver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(element.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockHccCountDatabase
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            service.MqExecutor.Setup(x => x.Count(It.IsAny<DateTime>())).Returns(10);
            _ = service.Execute();
            service.MqExecutor.Verify(x => x.Count(It.IsAny<DateTime>()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1, "not-a-date")]
        [InlineData(3, "")]
        [InlineData(4, "   ")]
        public void ComponentThrowsNullReferenceWhenParametersAreNotSet(int testId, string testDate = "")
        {
            var driver = new Mock<IWebDriver>();
            var parameters = testId switch
            {
                0 => null,
                _ => new MockParameters(testDate)
            };
            var service = new MockHccCountDatabase
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            Assert.Throws<NullReferenceException>(() => service.Execute());
        }
        private sealed class MockHccCountDatabase : HccCountDatabase
        {
            public MockHccCountDatabase()
            {
                HccService = MqExecutor.Object;
            }
            public Mock<IHccCountingService> MqExecutor { get; private set; } = new Mock<IHccCountingService>();

        }

        private sealed class MockParameters : DallasSearchProcess
        {
            public MockParameters(string startDate)
            {
                StartDate = startDate;
            }
        }
    }
}