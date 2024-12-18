using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    using Rex = Properties.Resources;
    public class TravisFetchCaseItemsTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 55;
            var service = new TravisFetchCaseItems();
            Assert.Equal(index, service.OrderId);
        }
        [Fact]
        public void ComponentCanExecute()
        {
            var error = Record.Exception(() =>
            {

                var driver = new Mock<IWebDriver>();
                var navigation = new Mock<INavigation>();
                var element = new Mock<IWebElement>();
                var parameters = new TravisSearchProcess();
                driver.Setup(x => x.Navigate()).Returns(navigation.Object);
                driver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(element.Object);
                navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
                // mock to retrieve a sample list of cases as html table
                element.Setup(m => m.GetAttribute(It.IsAny<string>())).Returns(Rex.dallas_case_items);
                var service = new MockTravisFetchCaseItems
                {
                    Parameters = parameters,
                    Driver = driver.Object
                };
                _ = service.Execute();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ComponentThrowingException(int target)
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new TravisSearchProcess();
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockTravisFetchCaseItems
            {
                Parameters = target != 1 ? parameters : null,
                Driver = target != 0 ? driver.Object : null
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        private sealed class MockTravisFetchCaseItems : TravisFetchCaseItems
        {
            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                MqExecutor.Setup(x => x.ExecuteScript(It.IsAny<string>()))
                    .Returns(Rex.travis_case_items);
                return MqExecutor.Object;
            }
        }
    }
}