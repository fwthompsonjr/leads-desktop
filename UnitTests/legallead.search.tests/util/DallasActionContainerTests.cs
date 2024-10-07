using LegalLead.PublicData.Search.Util;
using System.Linq;

namespace legallead.search.tests.util
{
    public class DallasActionContainerTests
    {
        [Fact]
        public void InstanceContainsContainer()
        {
            var error = Record.Exception(() =>
            {
                var item = DallasActionContainer.GetContainer;
                Assert.NotNull(item);
            });
            Assert.Null(error);
        }

        [Fact]
        public void InstanceGetFetchCollection()
        {
            var error = Record.Exception(() =>
            {
                var item = DallasActionContainer.GetContainer;
                var children = item.GetAllInstances<IDallasAction>();
                Assert.NotNull(children);
                Assert.Equal(6, children.Count());
            });
            Assert.Null(error);
        }
    }
}
