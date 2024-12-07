using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using System;
using System.Linq;
using Thompson.RecordSearch.Utility.Interfaces;

namespace legallead.search.tests.util
{
    public class ActionSettingContainerTests
    {
        [Fact]
        public void InstanceContainsContainer()
        {
            var error = Record.Exception(() =>
            {
                var item = ActionSettingContainer.GetContainer;
                Assert.NotNull(item);
            });
            Assert.Null(error);
        }

        [Fact]
        public void InstanceGetFetchCollection()
        {
            var error = Record.Exception(() =>
            {
                var item = ActionSettingContainer.GetContainer;
                var children = item.GetAllInstances<ISettingChangeModel>();
                Assert.NotNull(children);
                Assert.Equal(4, children.Count());
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("Boolean")]
        [InlineData("DateTime")]
        [InlineData("Numeric")]
        [InlineData("Text")]
        public void ServiceCanGetNamedInstance(string keyword)
        {
            var error = Record.Exception(() =>
            {
                var item = ActionSettingContainer.GetContainer;
                var actual = item.GetInstance<ISettingChangeModel>(keyword);
                Assert.NotNull(actual);
            });
            Assert.Null(error);
        }
    }
}