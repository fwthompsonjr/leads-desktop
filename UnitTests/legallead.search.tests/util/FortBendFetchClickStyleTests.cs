using Bogus;
using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace legallead.search.tests.util
{
    public class FortBendFetchClickStyleTests
    {
        [Fact]
        public void ComponentHasCorrectOrder()
        {
            const int index = 65;
            var service = new FortBendFetchClickStyle();
            Assert.Equal(index, service.OrderId);
        }
        [Fact]
        public void ComponentCanExecute()
        {
            var driver = new Mock<IWebDriver>();
            var element = new Mock<IWebElement>();
            var navigation = new Mock<INavigation>();
            var parameters = new DallasSearchProcess();
            driver.Setup(m => m.Navigate()).Returns(navigation.Object);
            driver.Setup(s => s.FindElement(It.IsAny<By>())).Returns(element.Object);
            navigation.Setup(m => m.Back());
            var service = new MockFortBendFetchClickStyle
            {
                Parameters = parameters,
                Driver = driver.Object
            };
            var error = Record.Exception(() => { _ = service.Execute(); });
            Assert.Null(error);
        }
        [Theory]
        [InlineData(1)]
        public void ComponentThrowingException(int target)
        {
            var driver = new Mock<IWebDriver>();
            var parameters = new DallasSearchProcess();
            var service = new MockFortBendFetchClickStyle
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

        private sealed class MockFortBendFetchClickStyle : FortBendFetchClickStyle
        {
            public Mock<IJavaScriptExecutor> MqExecutor { get; private set; } = new Mock<IJavaScriptExecutor>();
            public override IJavaScriptExecutor GetJavaScriptExecutor()
            {
                var faker = new Faker();
                var linkcount = faker.Random.Int(3, 5);
                var items = new List<string>();
                var person = new { name = "John Smith", address = "1234 Somewhere Dallas TX 75240", caseNo = "123abc" };
                var json = JsonConvert.SerializeObject(person);
                while(linkcount > 0) { items.Add(faker.Random.AlphaNumeric(8)); linkcount--; }
                var collection = JsonConvert.SerializeObject(items);

                MqExecutor.Setup(x => x.ExecuteScript(It.Is<string>(s => s.Contains("get person address")))).Returns(json);
                MqExecutor.Setup(x => x.ExecuteScript(It.Is<string>(s => s.Contains("find case detail links")))).Returns(collection);
                MqExecutor.Setup(x => x.ExecuteScript(It.Is<string>(s => s.Contains("click case detail links")))).Returns(string.Empty);
                return MqExecutor.Object;
            }
        }
    }
}