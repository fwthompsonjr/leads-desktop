using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LegalLead.Changed.Classes;
using System.Linq;

namespace LegalLead.Changed.UnitTests
{
    /// <summary>
    /// Summary description for CommandMapChangeIndexesTest
    /// </summary>
    [TestClass]
    public class CommandMapChangeIndexesTest
    {

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
            var sourceFile = @"D:\Alpha\LegalLead\LegalLead.Changed.UnitTests\data\temp-log.json";
            var command = new CommandMapChangeIndexes();
            command.SetSource(sourceFile);
            Assert.IsNotNull(command.Log);
        }

        [TestMethod]
        public void CanExecuteWithSource()
        {
            var sourceFile = @"D:\Alpha\LegalLead\LegalLead.Changed.UnitTests\bin\debug\data\temp-log.json";
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
