using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class DallasBeginNavigationTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 10;
            var service = new DallasBeginNavigation();
            Assert.Equal(index, service.OrderId);
        }

        [Fact]
        public void ComponentCanExecute()
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new DallasBeginNavigation
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            _ = service.Execute();
            navigation.Verify(x => x.GoToUrl(It.IsAny<Uri>()), Times.Once());
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
            var service = new DallasBeginNavigation
            {
                Parameters = target == 1 ? parameters : null,
                Driver = target == 0 ? driver.Object : null
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }
    }
}
