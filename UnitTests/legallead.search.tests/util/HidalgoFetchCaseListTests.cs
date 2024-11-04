using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class HidalgoFetchCaseListTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 55;
            var service = new HidalgoFetchCaseList();
            Assert.Equal(index, service.OrderId);
        }

        [Fact]
        public void ComponentCanExecute()
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            var element = new Mock<IWebElement>();
            var html = Properties.Resources.hidalgo_case_list;
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            driver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(element.Object);
            element.Setup(x => x.GetAttribute(It.IsAny<string>())).Returns(html);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new HidalgoFetchCaseList
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            _ = service.Execute();
            driver.Verify(x => x.FindElement(It.IsAny<By>()));
            element.Verify(x => x.GetAttribute(It.IsAny<string>()));
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
            var service = new HidalgoFetchCaseList
            {
                Parameters = target == 1 ? parameters : null,
                Driver = target == 0 ? driver.Object : null
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }
    }
}