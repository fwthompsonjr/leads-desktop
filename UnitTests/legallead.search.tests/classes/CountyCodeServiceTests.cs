using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Classes;

namespace legallead.search.tests.classes
{
    public class CountyCodeServiceTests
    {
        [Fact]
        public void ServceCanBeCreated()
        {
            var service = new CountyCodeService();
            Assert.NotNull(service);
        }
        [Fact]
        public void ServceHasMap()
        {
            var service = new CountyCodeService().Map;
            Assert.NotNull(service);
        }
        [Fact]
        public void ServceHasMappedWeb()
        {
            var service = new CountyCodeService().Map.Web;
            Assert.False(string.IsNullOrEmpty(service));
        }
        [Fact]
        public void ServceHasMapLandings()
        {
            var service = new CountyCodeService().Map.Landings;
            Assert.NotNull(service);            
            Assert.False(string.IsNullOrEmpty(service.County));
            Assert.False(string.IsNullOrEmpty(service.Login));
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(10, false)]
        [InlineData(11, false)]
        [InlineData(12, false)]
        [InlineData(13, false)]
        [InlineData(14, false)]
        [InlineData(2, true)]
        public void ServceCanGetWebAddress(int id, bool expected)
        {
            var service = new CountyCodeService();
            var address = service.GetWebAddress(id);
            var actual = string.IsNullOrEmpty(address);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(10, "account-login")]
        [InlineData(11, "set-county-login")]
        [InlineData(12, "change-password")]
        [InlineData(13, "set-county-permission")]
        [InlineData(14, "create-account")]
        public void ServceCanGetApiAddress(int id, string expected)
        {
            var service = new CountyCodeService();
            var address = service.GetWebAddress(id);
            Assert.Contains(expected, address);
        }
        [Theory]
        [InlineData("dallas", true)]
        [InlineData("Dallas", true)]
        [InlineData("collin", false)]
        public void ServceCanFindCountyByName(string name, bool expected)
        {
            var service = new CountyCodeService();
            var item = service.Find(name);
            var actual = item != null;
            Assert.Equal(expected, actual);
        }
    }
}
