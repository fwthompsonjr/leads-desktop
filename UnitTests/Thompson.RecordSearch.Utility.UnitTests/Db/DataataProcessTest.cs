using Harris.Criminal.Db.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Db;

namespace Thompson.RecordSearch.Utility.UnitTests.Db
{
    [TestClass]
    public class DataataProcessTest
    {
        [TestMethod]
        public async Task CanExecute_Download()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Assert.Inconclusive("This method to be executed in debug mode only.");
            }
            HccProcess reported = default;
            var process = new DownloadDataProcess();
            // The Progress<T> constructor captures our UI context,
            //  so the lambda will be run on the UI thread.
            var progress = new Progress<HccProcess>(percent =>
            {
                reported = percent;
            });

            // Processing is run on the thread pool.
            await Task.Run(() => process.Process(progress));
            Assert.IsNotNull(reported);
        }


        [TestMethod]
        public async Task CanExecute_DownloadCases()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Assert.Inconclusive("This method to be executed in debug mode only.");
            }
            HccProcess reported = default;
            var process = new DownloadCaseProcess();
            // The Progress<T> constructor captures our UI context,
            //  so the lambda will be run on the UI threa-
            var progress = new Progress<HccProcess>(percent =>
            {
                reported = percent;
            });

            // Processing is run on the thread pool.
            await Task.Run(() => process.Process(progress));
            Assert.IsNotNull(reported);
        }
    }
}
