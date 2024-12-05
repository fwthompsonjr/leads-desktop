using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using System;
using System.Linq;
using Thompson.RecordSearch.Utility.Interfaces;

namespace legallead.search.tests.util
{
    public class ActionHccContainerTests
    {
        [Fact]
        public void InstanceContainsContainer()
        {
            var error = Record.Exception(() =>
            {
                var item = ActionHccContainer.GetContainer;
                Assert.NotNull(item);
            });
            Assert.Null(error);
        }

        [Fact]
        public void InstanceGetFetchCollection()
        {
            var error = Record.Exception(() =>
            {
                var item = ActionHccContainer.GetContainer;
                var children = item.GetAllInstances<ICountySearchAction>();
                Assert.NotNull(children);
                Assert.Equal(5, children.Count());
            });
            Assert.Null(error);
        }
        [Theory]
        [InlineData(typeof(IHttpService))]
        [InlineData(typeof(ICountyCodeService))]
        [InlineData(typeof(ICountyCodeReader))]
        [InlineData(typeof(IHccReadingService))]
        [InlineData(typeof(IHccWritingService))]
        [InlineData(typeof(IHccCountingService))]
        public void ServiceCanGetTypedInstance(Type type)
        {
            var error = Record.Exception(() =>
            {
                var item = ActionHccContainer.GetContainer;
                var actual = item.GetInstance(type);
                Assert.NotNull(actual);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("count")]
        [InlineData("begin")]
        [InlineData("download-monthly")]
        [InlineData("download-summary")]
        [InlineData("get-case-list")]
        public void ServiceCanGetNamedInstance(string keyword)
        {
            var error = Record.Exception(() =>
            {
                var item = ActionHccContainer.GetContainer;
                var actual = item.GetInstance<ICountySearchAction>(keyword);
                Assert.NotNull(actual);
            });
            Assert.Null(error);
        }
    }
}