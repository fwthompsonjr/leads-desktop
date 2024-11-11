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
        [InlineData(2, true)]
        public void ServceCanGetWebAddress(int id, bool expected)
        {
            var service = new CountyCodeService();
            var address = service.GetWebAddress(id);
            var actual = string.IsNullOrEmpty(address);
            Assert.Equal(expected, actual);
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
