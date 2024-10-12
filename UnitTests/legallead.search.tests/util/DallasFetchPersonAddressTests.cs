using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;
using Thompson.RecordSearch.Utility.Dto;

namespace legallead.search.tests.util
{
    public class DallasFetchPersonAddressTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 80;
            var service = new DallasFetchPersonAddress();
            Assert.Equal(index, service.OrderId);
        }

        [Fact]
        public void ComponentCanExecute()
        {
            var error = Record.Exception(() =>
            {
                var driver = new Mock<IWebDriver>();
                var navigation = new Mock<INavigation>();
                driver.Setup(x => x.Navigate()).Returns(navigation.Object);
                navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
                var service = new DallasFetchPersonAddress
                {
                    Parameters = new DallasSearchProcess(),
                    Driver = driver.Object,
                    Dto = new CaseItemDto()
                };
                _ = service.Execute();
            });
            Assert.Null(error);

        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ComponentThrowingException(int target)
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new DallasFetchPersonAddress
            {
                Parameters = target != 1 ? parameters : null,
                Driver = target != 0 ? driver.Object : null,
                Dto = target != 2 ? new CaseItemDto() : null
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }
    }
}