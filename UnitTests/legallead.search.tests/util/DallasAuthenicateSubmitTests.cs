using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace legallead.search.tests.util
{
    public class DallasAuthenicateSubmitTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 6;
            var service = new MockDallasAuthenicateSubmit();
            Assert.Equal(index, service.OrderId);
        }
        [Fact]
        public void ComponentCanExecute()
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            var element = new Mock<IWebElement>();
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            driver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(element.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockDallasAuthenicateSubmit
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
            driver.SetupSequence(x => x.Title)
                .Returns("abc")
                .Returns("abc")
                .Returns("abd");
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockDallasAuthenicateSubmit
            {
                Parameters = target != 1 ? parameters : null,
                Driver = target != 0 ? driver.Object : null
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        private sealed class MockDallasAuthenicateSubmit : DallasAuthenicateSubmit
        {
            public MockDallasAuthenicateSubmit() : base(mockReader.Object)
            {
                var http = new HttpService();
                var svc = new CountyCodeService();
                var actual = new CountyCodeReaderService(http, svc);
                var response = actual.GetCountyCode(60);
                mockReader.Setup(s => s.GetCountyCode(It.IsAny<int>())).Returns(response);
                mockReader.Setup(s => s.GetCountyCode(It.IsAny<string>())).Returns(response);
            }

            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                MqExecutor.SetupSequence(x => x.ExecuteScript(It.IsAny<string>()))
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);
                return MqExecutor.Object;
            }
            private static readonly Mock<ICountyCodeReader> mockReader = new();
        }
    }
}