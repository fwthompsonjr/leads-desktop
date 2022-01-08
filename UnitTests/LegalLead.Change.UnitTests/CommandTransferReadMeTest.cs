using LegalLead.Changed.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace LegalLead.Changed.UnitTests
{
    [TestClass]
    public class CommandTransferReadMeTest
    {
        private static string _srcDirectory;
        private static string SrcDirectoryName => _srcDirectory ?? (_srcDirectory = SrcDir());
        static readonly string srcFile = $"{SrcDirectoryName}\\data\\temp-log.json";

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

        private static string SrcDir()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return Path.GetDirectoryName(assembly.Location);
        }

    }
}
