using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;
using System.Threading;

namespace legallead.search.tests.util
{
    public class TravisRequestCaptchaTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 20;
            var service = new TravisRequestCaptcha();
            Assert.Equal(index, service.OrderId);
        }
        [Fact]
        public void ComponentCanExecute()
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new TravisSearchProcess();
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockTravisRequestCaptcha
            {
                Parameters = parameters,
                Driver = driver.Object,
                PromptUser = MockUserPrompt
            };
            _ = service.Execute();
            service.MqExecutor.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.AtMost(3));
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ComponentThrowingException(int target)
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new TravisSearchProcess();
            driver.Setup(x => x.Navigate()).Returns(navigation.Object);
            navigation.Setup(x => x.GoToUrl(It.IsAny<Uri>())).Verifiable();
            var service = new MockTravisRequestCaptcha
            {
                Parameters = target != 1 ? parameters : null,
                Driver = target != 0 ? driver.Object : null,
                PromptUser = target != 2 ? MockUserPrompt : null
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        private static bool MockUserPrompt()
        {
            Thread.Sleep(100);
            return true;
        }

        private sealed class MockTravisRequestCaptcha : TravisRequestCaptcha
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