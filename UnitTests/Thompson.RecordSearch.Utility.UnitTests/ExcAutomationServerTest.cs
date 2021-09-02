using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.Tests
{
    [TestClass]
    public class ExcAutomationServerTest
    {
        [TestMethod]
        [TestCategory("Excel.Automation.Tests")]
        public void CanOpenExcel()
        {
            ExcAutomationServer.Open("");
        }
    }
}
