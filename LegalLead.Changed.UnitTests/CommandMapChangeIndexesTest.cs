using LegalLead.Changed.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LegalLead.Changed.UnitTests
{
    /// <summary>
    /// Summary description for CommandMapChangeIndexesTest
    /// </summary>
    [TestClass]
    public class CommandMapChangeIndexesTest
    {
        protected const string sourceFile = @"D:\Alpha\LegalLead\LegalLead.Changed.UnitTests\bin\debug\data\temp-log.json";

        [TestMethod]
        public void CanInit()
        {
            var command = new CommandMapChangeIndexes();
            Assert.IsNotNull(command);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CanExecuteNeedsSourceFile()
        {
            var command = new CommandMapChangeIndexes();
            command.Execute();
        }
        [TestMethod]
        public void CanLoadSource()
        {
            var command = new CommandMapChangeIndexes();
            command.SetSource(sourceFile);
            Assert.IsNotNull(command.Log);
        }

        [TestMethod]
        public void CanExecuteWithSource()
        {
            var command = new CommandMapChangeIndexes();
            command.SetSource(sourceFile);
            command.Execute();
            // after execute
            // we expect that NO changes exist without a changeId
            var expected = command.Log.Changes.Count;
            var actual = command.Log.Changes.ToList()
                .Count(a => !string.IsNullOrEmpty(a.ChangeId));
            Assert.AreEqual(expected, actual);
        }
    }
}
