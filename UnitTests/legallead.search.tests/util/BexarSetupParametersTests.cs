using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class BexarSetupParametersTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 30;
            var service = new BexarSetupParameters();
            Assert.Equal(index, service.OrderId);
        }


        [Fact]
        public void ComponentHasBaseExecutor()
        {
            var service = new MockBexarSetupParameters();
            var executor = service.GetBaseExecutor();
            Assert.Null(executor);
        }

        [Theory]
        [InlineData("JUSTICE")]
        [InlineData("DISTRICT")]
        [InlineData("COUNTY")]
        public void ComponentCanExecute(string locator)
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            var startDt = DateTime.Now;
            var endingDt = DateTime.Now.AddDays(3);
            parameters.SetSearchParameters(startDt, endingDt, locator);
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockBexarSetupParameters
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            _ = service.Execute();
            service.MqExecutor.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.AtLeast(2));
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
            var service = new MockBexarSetupParameters
            {
                Parameters = target != 1 ? parameters : null,
                Driver = target != 0 ? driver.Object : null
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        private sealed class MockBexarSetupParameters : BexarSetupParameters
        {
            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                const string request = "return document.readyState";
                const string response = "complete";
                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s => !s.Equals(request))))
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);

                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s => s.Equals(request))))
                    .Returns(0)
                    .Returns(string.Empty)
                    .Returns(response);
                return MqExecutor.Object;
            }
            public IJavaScriptExecutor GetBaseExecutor()
            {
                return base.GetJavaScriptExecutor();
            }
        }
    }
}