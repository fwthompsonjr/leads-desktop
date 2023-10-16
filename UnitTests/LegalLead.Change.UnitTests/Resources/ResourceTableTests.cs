using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LegalLead.Changed.UnitTests.Resources
{
    using LegalLead.Resources;
    using System.IO;

    [TestClass]
    public class ResourceTableTests
    {
        [TestMethod]
        public void CanGetAppFolder()
        {
            var folder = ResourceTable.AppFolder;
            Assert.IsFalse(string.IsNullOrEmpty(folder));
            Assert.IsTrue(Directory.Exists(folder));
        }


        [TestMethod]
        public void CanGetResourceFileName()
        {
            var fileName = ResourceTable.ResourceFileName;
            Console.WriteLine("Resource File: {0}", fileName);
            Assert.IsFalse(string.IsNullOrEmpty(fileName));
            Assert.IsTrue(File.Exists(fileName));
        }

        [TestMethod]
        public void CanGetResourceTypeNames()
        {
            var type = typeof(ResourceType);
            var names = Enum.GetNames(type);
            Assert.IsNotNull(names);
            Assert.IsTrue(names.Length > 0);
        }

        [TestMethod]
        public void CanGetResourceKeyIndexNames()
        {
            var type = typeof(ResourceKeyIndex);
            var names = Enum.GetNames(type);
            Assert.IsNotNull(names);
            Assert.IsTrue(names.Length > 0);
        }

        [TestMethod]
        public void CanGetResourcesByType()
        {
            var type = typeof(ResourceType);
            var names = Enum.GetNames(type);
            var id = (new Random(DateTime.Now.Millisecond)).Next(0, names.Length - 1);
            var resourceType = (ResourceType)Enum.Parse(type, names[id]);
            var resources = ResourceTable.GetResources(resourceType);
            Assert.IsNotNull(resources);
            Assert.IsTrue(resources.Count() > 0);
        }


        [TestMethod]
        public void CanGetResourcesByTypeAndKeyIndex()
        {
            var type = typeof(ResourceType);
            var keyType = typeof(ResourceKeyIndex);
            var names = Enum.GetNames(type);
            var keys = Enum.GetNames(keyType);
            var random = new Random(DateTime.Now.Millisecond);
            var id = random.Next(0, names.Length - 1);
            var keyIndex = random.Next(0, keys.Length - 1);
            var resourceType = (ResourceType)Enum.Parse(type, names[id]);
            var resourceKey = (ResourceKeyIndex)Enum.Parse(keyType, keys[keyIndex]);
            var resources = ResourceTable.GetResources(resourceType, resourceKey);
            Assert.IsNotNull(resources);
        }


        [TestMethod]
        public void CanGetResourceTextByTypeAndKeyIndex()
        {
            var type = typeof(ResourceType);
            var keyType = typeof(ResourceKeyIndex);
            var names = Enum.GetNames(type);
            var keys = Enum.GetNames(keyType);
            var random = new Random(DateTime.Now.Millisecond);
            var id = random.Next(0, names.Length - 1);

            var resourceType = (ResourceType)Enum.Parse(type, names[id]);

            var resources = ResourceTable.GetResources(resourceType).ToList();
            Assert.IsNotNull(resources);
            var keyIndex = random.Next(0, resources.Count - 1);
            var keyItem = resources[keyIndex].KeyIndex;
            var resourceKey = (ResourceKeyIndex)keyItem;
            var actual = ResourceTable.GetText(resourceType, resourceKey);
            Assert.IsFalse(string.IsNullOrEmpty(actual));
        }
    }
}
