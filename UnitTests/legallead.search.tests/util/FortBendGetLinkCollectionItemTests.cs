using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;

namespace legallead.search.tests.util
{
    public class FortBendGetLinkCollectionItemTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 75;
            var service = new FortBendGetLinkCollectionItem();
            Assert.Equal(index, service.OrderId);
        }
        [Fact]
        public void ComponentHasLinkItemId()
        {
            const int index = 75;
            var service = new FortBendGetLinkCollectionItem
            {
                LinkItemId = index
            };
            Assert.Equal(index, service.LinkItemId);
        }
        [Fact]
        public void ComponentCanExecute()
        {
            var driver = new Mock<IWebDriver>();
            var parameters = new DallasSearchProcess();
            var service = new MockFortBendGetLinkCollectionItem
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            var error = Record.Exception(() => { _ = service.Execute(); });
            Assert.Null(error);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ComponentThrowingException(int target)
        {
            var driver = new Mock<IWebDriver>();
            var parameters = new DallasSearchProcess();
            var service = new MockFortBendGetLinkCollectionItem
            {
                Parameters = GetItemOrDefault(target == 0, parameters),
                Driver = GetItemOrDefault(target == 1, driver.Object)
            };
            Assert.Throws<NullReferenceException>(() => { _ = service.Execute(); });
        }

        private static T GetItemOrDefault<T>(bool isNull, T defaultValue)
        {
            if (isNull) return default;
            return defaultValue;
        }

        private sealed class MockFortBendGetLinkCollectionItem : FortBendGetLinkCollectionItem
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