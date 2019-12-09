using System;
using LegalLead.Changed.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LegalLead.Changed.UnitTests
{
    [TestClass]
    public class CommandTransferReadMeTest
    {
        const string srcFile = @"D:\Alpha\LegalLead\LegalLead.Changed.UnitTests\data\temp-log.json";

        [TestMethod]
        public void CanLoadTestLog()
        {
            var sourceFile = srcFile;
            var command = new CommandTransferReadMe();
            command.SetSource(sourceFile);
            Assert.IsNotNull(command.Log);
        }
        [TestMethod]
        public void CanExecuteTestLog()
        {
            var sourceFile = srcFile;
            var command = new CommandTransferReadMe();
            command.SetSource(sourceFile);
            command.Execute();
        }


    }
}
