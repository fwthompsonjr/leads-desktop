using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;
using System.Diagnostics;

namespace legallead.search.tests.util
{
    using Rex = Properties.Resources;
    public class DallasFetchCaseItemsTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 55;
            var service = new DallasFetchCaseItems();
            Assert.Equal(index, service.OrderId);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ComponentCanExecute(bool noDataResponse)
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var element = new Mock<IWebElement>();
            var parameters = new DallasSearchProcess();
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            driver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(element.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            element.Setup(m => m.GetAttribute(It.IsAny<string>())).Returns(Rex.dallas_case_items);
            var service = new MockDallasFetchCaseItems(noDataResponse)
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            _ = service.Execute();
            if (noDataResponse)
            {
                element.Verify(m => m.GetAttribute(It.IsAny<string>()), Times.Never);
            } 
            else
            {
                element.Verify(m => m.GetAttribute(It.IsAny<string>()), Times.Once);
            }
        }
        [Fact]
        public void ComponentCanExecuteWhenElementNotFound()
        {
            if (!Debugger.IsAttached) return;
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            IWebElement element = null;
            var parameters = new DallasSearchProcess();
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            driver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(element);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockDallasFetchCaseItems
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            var actual = service.Execute();
            if (actual is string str)
            {
                Assert.True(string.IsNullOrEmpty(str));
            }
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
            var service = new MockDallasFetchCaseItems
            {
                Parameters = target != 1 ? parameters : null,
                Driver = target != 0 ? driver.Object : null
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        private sealed class MockDallasFetchCaseItems : DallasFetchCaseItems
        {
            private readonly bool bNoCaseResponse;
            public MockDallasFetchCaseItems(bool noCaseResponse = false)
            {
                bNoCaseResponse = noCaseResponse;
            }
            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                const string find = "no cases match your search";
                MqExecutor.Setup(x => x.ExecuteScript(It.Is<string>(s => s.Contains(find))))
                    .Returns(bNoCaseResponse);

                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s => !s.Contains(find))))
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);
                return MqExecutor.Object;
            }
        }
    }
}