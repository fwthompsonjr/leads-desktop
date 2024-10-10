using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.UnitTests.Services
{
    [TestClass]
    public class CountyCodeServiceTests
    {
        [TestMethod]
        public void ServiceCanBeConstructed()
        {
            var service = new CountyCodeService();
            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void ServiceContainsMap()
        {
            var service = new CountyCodeService();
            Assert.IsNotNull(service.Map);
            Assert.IsNotNull(service.Map.Counties);
            Assert.IsTrue(service.Map.Counties.Any());
        }

        [TestMethod]
        [DataRow(0, true)]
        [DataRow(60, false)]
        public void ServiceCanFindCodeById(int countyId, bool isNull)
        {
            var service = new CountyCodeService();
            var actual = service.Find(countyId);
            Assert.AreEqual(isNull, actual == null);
        }

        [TestMethod]
        [DataRow("", true)]
        [DataRow("missing", true)]
        [DataRow("dallas", false)]
        public void ServiceCanFindCodeByName(string name, bool isNull)
        {
            var service = new CountyCodeService();
            var actual = service.Find(name);
            Assert.AreEqual(isNull, actual == null);
        }
    }
}
