using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.search.tests.helpers
{
    public class SessionSettingPersistenceTests
    {
        [Fact]
        public void ServiceCanInitialize()
        {
            var error = Record.Exception(() =>
            {
                var svc = new SessionSettingPersistence();
                svc.Initialize();
            });
            Assert.Null(error);
        }
        [Fact]
        public void ServiceCanGetList()
        {
            var error = Record.Exception(() =>
            {
                var svc = new SessionSettingPersistence();
                var list = svc.GetList<UserSettingChangeModel>();
                Assert.NotEmpty(list);
            });
            Assert.Null(error);
        }
        [Fact]
        public void ServiceCanGetView()
        {
            var error = Record.Exception(() =>
            {
                var svc = new SessionSettingPersistence();
                var list = svc.GetList<UserSettingChangeViewModel>();
                Assert.NotEmpty(list);
            });
            Assert.Null(error);
        }
        [Theory]
        [InlineData("", "", false)]
        [InlineData("key", "", false)]
        [InlineData("", "value", false)]
        [InlineData("search", "End Date:", true)]
        public void ServiceCanChangeItem(string key, string value, bool expected)
        {
            var error = Record.Exception(() =>
            {
                var svc = new SessionSettingPersistence();
                var model = new UserSettingChangeViewModel { Category = key, Name = value };
                var actual = svc.Change(model);
                Assert.Equal(expected, actual);
            });
            Assert.Null(error);
        }
    }
}
