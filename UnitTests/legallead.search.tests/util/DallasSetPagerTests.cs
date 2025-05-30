﻿using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;

namespace legallead.search.tests.util
{
    public class DallasSetPagerTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 50;
            var service = new DallasSetPager();
            Assert.Equal(index, service.OrderId);
        }
        [Fact]
        public void ComponentCanExecute()
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            var element = new Mock<IWebElement>();
            var items = new[]
            {
                element.Object,
                element.Object,
                element.Object,
                element.Object,
                element.Object,
                element.Object,
                element.Object,
                element.Object,
                element.Object,
                element.Object,
                element.Object,
            };
            var collection = new ReadOnlyCollection<IWebElement>(items);
            element.SetupGet(x => x.Text).Returns("Status");
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            driver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(element.Object);
            driver.Setup(x => x.FindElements(It.IsAny<By>())).Returns(collection);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockDallasSetPager
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
        public void ComponentThrowingException(int target)
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockDallasSetPager
            {
                Parameters = target != 1 ? parameters : null,
                Driver = target != 0 ? driver.Object : null
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        private sealed class MockDallasSetPager : DallasSetPager
        {
            public MockDallasSetPager()
            {
                IsTesting = true;
            }
            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                const string load = ".filter(x => x.innerText == 'Status');";
                const string isSorted = "return statusSort.isSorted();";
                const string clicked = "return statusSort.click();";
                const string csLoad = ".filter(x => x.innerText == 'Type');";
                const string csIsSorted = "return typeSort.isSorted();";
                const string csClicked = "return typeSort.click();";

                MqExecutor.Setup(x => x.ExecuteScript(It.Is<string>(s => s.Contains(clicked))))
                    .Returns(true);

                MqExecutor.Setup(x => x.ExecuteScript(It.Is<string>(s => s.Contains(load))))
                    .Returns(true);

                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s => s.Contains(isSorted))))
                    .Returns("")
                    .Returns(false)
                    .Returns(true);


                MqExecutor.Setup(x => x.ExecuteScript(It.Is<string>(s => s.Contains(csClicked))))
                    .Returns(true);

                MqExecutor.Setup(x => x.ExecuteScript(It.Is<string>(s => s.Contains(csLoad))))
                    .Returns(true);

                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s => s.Contains(csIsSorted))))
                    .Returns("")
                    .Returns(false)
                    .Returns(true);

                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s =>
                !s.Contains(isSorted) && !s.Contains(load) && !s.Contains(clicked))))
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);
                return MqExecutor.Object;
            }
        }
    }
}