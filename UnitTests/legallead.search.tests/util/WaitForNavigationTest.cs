using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class WaitForNavigationTest
    {
        [Fact]
        public void ComponentCanExecute()
        {
            var driver = new Mock<IWebDriver>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasAttendedProcess();
            var startDt = DateTime.Now;
            var endingDt = DateTime.Now.AddDays(3);
            parameters.Search(startDt, endingDt, "JUSTICE");
            var service = new MockWaitForNavigation
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            _ = service.Execute();
            service.MqExecutor.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.AtMost(3));
        }

        private sealed class MockWaitForNavigation : BaseDallasSearchAction
        {
            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public override object Execute()
            {
                WaitForNavigation();
                return true;
            }
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                const string request = "return document.readyState";
                MqExecutor.SetupSequence(x => x.ExecuteScript(It.Is<string>(s => s.Equals(request))))
                    .Returns("no")
                    .Returns("no")
                    .Returns("complete");
                return MqExecutor.Object;
            }
        }
    }
}
