using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class BexarSetPagerTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 50;
            var service = new BexarSetPager();
            Assert.Equal(index, service.OrderId);
        }
        [Fact]
        public void ComponentCanExecute()
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            var element = new Mock<IWebElement>();
            driver.Setup(x => x.FindElement(It.Is<By>(x => x == By.Id("hearingResultsGrid")))).Returns(element.Object);
            driver.Setup(x => x.FindElement(It.Is<By>(x => x == By.TagName("ul")))).Returns(element.Object);
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockBexarSetPager
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            _ = service.Execute();
            service.MqExecutor.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.AtLeastOnce());
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ComponentThrowingException(int target)
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            var element = new Mock<IWebElement>();
            IWebElement nonElement = default;

            if (target < 2)
            {
                driver.Setup(x => x.FindElement(It.Is<By>(x => x == By.Id("hearingResultsGrid")))).Returns(element.Object);
                driver.Setup(x => x.FindElement(It.Is<By>(x => x == By.TagName("ul")))).Returns(element.Object);
            }
            if (target == 2)
            {
                driver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(nonElement);
            }
            if (target == 3)
            {
                driver.Setup(x => x.FindElement(It.Is<By>(x => x == By.Id("hearingResultsGrid")))).Returns(element.Object);
                driver.Setup(x => x.FindElement(It.Is<By>(x => x == By.TagName("ul")))).Returns(nonElement);
            }
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockBexarSetPager
            {
                Parameters = GetItemOrDefault(target == 0, parameters),
                Driver = GetItemOrDefault(target == 1, driver.Object)
            };
            if (target < 2)
            {
                Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
            }
            else
            {
                _ = service.Execute();
                driver.Verify(x => x.FindElement(It.IsAny<By>()));
            }
        }

        private static T GetItemOrDefault<T>(bool isNull, T defaultValue)
        {
            if (isNull) return default;
            return defaultValue;
        }

        private sealed class MockBexarSetPager : BexarSetPager
        {
            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                MqExecutor.SetupSequence(x => x.ExecuteScript(It.IsAny<string>()))
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);
                return MqExecutor.Object;
            }
        }
    }
}