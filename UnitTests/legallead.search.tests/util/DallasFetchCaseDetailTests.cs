using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class DallasFetchCaseDetailTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 60;
            var service = new DallasFetchCaseDetail();
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
            var service = new MockDallasFetchCaseDetail
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            _ = service.Execute();
            service.MqExecutor.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.AtMost(3));
        }

        [Theory]
        [InlineData("string", false)]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(true, false, false)]
        public void ComponentCanCalcNoCount(object uiResponse, bool expected, bool useMock = true)
        {
            var sut = new MockItemCounter();
            var actual = sut.GetNoCount(uiResponse, useMock);
            Assert.Equal(expected, actual);
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
            var service = new MockDallasFetchCaseDetail
            {
                Parameters = target != 1 ? parameters : null,
                Driver = target != 0 ? driver.Object : null
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        private sealed class MockDallasFetchCaseDetail : DallasFetchCaseDetail
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
        private sealed class MockItemCounter : DallasFetchCaseItems
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


            public bool GetNoCount(object expected, bool hasExecutor = true)
            {
                if (!hasExecutor)
                {
                    return IsNoCount(null);
                }

                MqExecutor.Setup(x => x.ExecuteScript(It.IsAny<string>())).Returns(expected);
                var actual = IsNoCount(MqExecutor.Object);
                return actual;
            }

        }
    }
}