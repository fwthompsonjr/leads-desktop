using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.UnitTests.Dallas
{
    [TestClass]
    public class ScriptingTests
    {
        [TestMethod]
        public void DoesScriptFileExist()
        {
            var obj = DallasScriptHelper.GetScriptFileName;
            var exists = File.Exists(obj);
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void CanGetScriptContent()
        {
            var obj = DallasScriptHelper.GetScriptContent;
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void CanGetScriptFileName()
        {
            var obj = DallasScriptHelper.GetScriptFileName;
            Assert.IsNotNull(obj);
        }
    }
}