using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Tests
{
    [TestClass]
    public class CollinCountyNavigationTests
    {
        [TestMethod]
        public void CanGetCaseTypes()
        {
            var caseTypes = CaseTypeSelectionDto.GetDto("collinCountyCaseType");
            Assert.IsNotNull(caseTypes);
        }

        [TestMethod]
        public void CanGetProbateSelection()
        {
            var caseTypes = CaseTypeSelectionDto.GetDto("collinCountyCaseType");
            Assert.IsNotNull(caseTypes);
            Assert.IsTrue(caseTypes.DropDowns.Any(x => x.Id == 1));
            var dropDown = caseTypes.DropDowns.First(x => x.Id == 1);
            Assert.AreEqual(dropDown.Name, "probate courts");

        }
    }
}
