using LegalLead.PublicData.Search.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using Thompson.RecordSearch.Utility.Web;

namespace Harris.Criminal.UnitTests
{
    [TestClass]
    public class HarrisCriminalUpdateTests
    {



        [TestMethod]
        [TestCategory("Integration Only")]
        public void Update_Will_Download_Filings()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Assert.Inconclusive("This method to be executed in debug mode only.");
            }
            var downloadFile = string.Empty; // HarrisCriminalData.DownloadFileName;
            var timeoutDate = DateTime.Now.Add(TimeSpan.FromMinutes(5));
            if (File.Exists(downloadFile))
            {
                File.Delete(downloadFile);
            }
            HarrisCriminalUpdate.Update();
            while (DateTime.Now < timeoutDate)
            {
                Thread.Sleep(250);
                if (HarrisCriminalUpdate.IsDataReady) break;
                if (File.Exists(downloadFile)) break;
            }
            Assert.IsTrue(File.Exists(downloadFile));
        }
    }
}
