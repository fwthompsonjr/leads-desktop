using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.UnitTests.Dallas
{
    [TestClass]
    public class ScriptingTests
    {

        [TestMethod]
        public void DoesNavigationUriExist()
        {
            var obj = DallasScriptHelper.NavigationSteps;
            var item = obj.Find(x => x.ActionName == "navigate");
            Assert.IsNotNull(item);
            var uri = item.Locator.Query;
            Assert.IsFalse(string.IsNullOrEmpty(uri));
        }

        [TestMethod]
        public void DoNavigationStepsExist()
        {
            var obj = DallasScriptHelper.NavigationSteps;
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Count > 0);
        }

        [TestMethod]
        public void DoesScriptCollectionExist()
        {
            var obj = DallasScriptHelper.ScriptCollection;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Count > 0);
        }

        [TestMethod]
        [DataRow("is captcha needed")]
        [DataRow("set start and end date")]
        [DataRow("select max rows per page")]
        [DataRow("get case list")]
        [DataRow("get case style")]
        public void DoesScriptCollectionContainKey(string keyname)
        {
            var obj = DallasScriptHelper.ScriptCollection;
            var exists = obj.TryGetValue(keyname, out var _);
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void DoesScriptBlockExist()
        {
            var obj = DallasScriptHelper.Scripts;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Count > 0);
        }

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
        public void CanGetJsonContent()
        {
            var obj = DallasScriptHelper.GetJsonContent;
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void CanGetScriptFileName()
        {
            var obj = DallasScriptHelper.GetScriptFileName;
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void DoesJsonFileExist()
        {
            var obj = DallasScriptHelper.GetJsonFileName;
            var exists = File.Exists(obj);
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void CanGetJsonFileName()
        {
            var obj = DallasScriptHelper.GetJsonFileName;
            Assert.IsNotNull(obj);
        }
    }
}