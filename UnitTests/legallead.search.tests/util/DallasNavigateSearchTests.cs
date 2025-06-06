﻿using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class DallasNavigateSearchTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 40;
            var service = new MockDallasNavigateSearch();
            Assert.Equal(index, service.OrderId);
        }

        [Fact]
        public void ComponentCanExecute()
        {
            var driver = new Mock<IWebDriver>();
            var element = new Mock<IWebElement>();
            driver.Setup(m => m.FindElement(It.IsAny<By>())).Returns(element.Object);
            element.Setup(m => m.Click()).Verifiable();
            var parameters = new DallasSearchProcess();
            var service = new MockDallasNavigateSearch
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            _ = service.Execute();
            service.MqExecutor.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.AtLeast(1));
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ComponentThrowingException(int target)
        {
            var driver = new Mock<IWebDriver>();
            var parameters = new DallasSearchProcess();
            var service = new MockDallasNavigateSearch
            {
                Parameters = target != 1 ? parameters : null,
                Driver = target != 0 ? driver.Object : null
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        private sealed class MockDallasNavigateSearch : DallasNavigateSearch
        {
            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                const string request = "return document.readyState";
                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s => s.Equals(request))))
                    .Returns("no")
                    .Returns("no")
                    .Returns("complete");

                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s => !s.Equals(request))))
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);
                return MqExecutor.Object;
            }
        }
    }
}