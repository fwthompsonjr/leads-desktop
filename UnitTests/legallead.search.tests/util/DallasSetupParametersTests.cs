﻿using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class DallasSetupParametersTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 30;
            var service = new DallasSetupParameters();
            Assert.Equal(index, service.OrderId);
        }

        [Fact]
        public void ComponentCanExecute()
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var options = new Mock<IOptions>();
            var timeouts = new Mock<ITimeouts>();
            var parameters = new DallasSearchProcess();
            var startDt = DateTime.Now;
            var endingDt = DateTime.Now.AddDays(3);
            parameters.SetSearchParameters(startDt, endingDt, "JUSTICE");
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            driver.Setup(x => x.Manage()).Returns(options.Object);
            options.Setup(x => x.Timeouts()).Returns(timeouts.Object);
            timeouts.Setup(x => x.PageLoad).Returns(TimeSpan.FromSeconds(1));
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockDallasSetupParameters
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            _ = service.Execute();
            service.MqExecutor.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.AtMost(3));
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3, "")]
        [InlineData(4, "NOTMAPPED")]
        [InlineData(4, "COUNTY", false)]
        [InlineData(4, "DISTRICT", true, false)]
        public void ComponentThrowingException(int target, string searchType = "JUSTICE", bool hasStartDate = true, bool hasEndingDate = true)
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            DateTime? startDt = hasStartDate ? DateTime.Now : null;
            DateTime? endingDt = hasEndingDate ? DateTime.Now.AddDays(3) : null;
            parameters.SetSearchParameters(startDt, endingDt, searchType);
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockDallasSetupParameters
            {
                Parameters = target != 1 ? parameters : null,
                Driver = target != 0 ? driver.Object : null
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        private sealed class MockDallasSetupParameters : DallasSetupParameters
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