using LegalLead.Changed.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LegalLead.Changed.UnitTests
{
    [TestClass]
    public class CommandTransferReadMeTest
    {
        protected const string projectFolder = @"D:\Alpha\LegalLead\UnitTests\LegalLead.Change.UnitTests\bin\debug\net472";
        static readonly string srcFile = $"{projectFolder}\\data\\temp-log.json";

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
