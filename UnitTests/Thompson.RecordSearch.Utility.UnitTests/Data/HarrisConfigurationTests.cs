using LegalLead.PublicData.Search.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.UnitTests.Data
{
    [TestClass]
    public class HarrisConfigurationTests
    {
        [TestMethod]
        [TestCategory("Harris.Criminal.Configuration")]
        public void CanGetJsonString()
        {
            var js = SettingsManager.CustomSettings;
            Assert.IsFalse(string.IsNullOrEmpty(js));
        }

        [TestMethod]
        [TestCategory("Harris.Criminal.Configuration")]
        public void CanDeserializeJson()
        {
            var obj = HccConfiguration.Load();
            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.Dropdown);
            Assert.IsFalse(obj.Dropdown.IsEnabled);
            Assert.AreEqual(40, obj.Dropdown.Index);
        }
    }
}
