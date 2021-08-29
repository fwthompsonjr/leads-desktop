using Harris.Criminal.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Web;

namespace Thompson.RecordSearch.Utility.Tests
{
    [TestClass]
    public class HarrisCriminalCaseStyleTests : TestingBase
    {
        private int MxCaseNumbers = 10;
        private List<HarrisCaseSearchDto> CaseNumbers;

        [TestInitialize]
        public void Setup()
        {
            if (CaseNumbers == null)
            {
                Startup.Downloads.Read();
                var datalist = Startup.Downloads.DataList.FirstOrDefault();
                var dtos = datalist.Data.Select(x =>
                    new HarrisCaseSearchDto
                    {
                        CaseNumber = x.CaseNumber,
                        Court = x.Court,
                        DateFiled = x.FilingDate.ToExactDateString("yyyyMMdd", string.Empty)
                    });
                var list = dtos.GroupBy(x => x.UniqueIndex).Select(x => x.FirstOrDefault());
                CaseNumbers = list.Take(MxCaseNumbers).ToList();
            }
        }

        [TestMethod]
        public void Download_HasACorrectTarget()
        {
            var obj = new HarrisCriminalCaseStyle();
            var folder = obj.DownloadFolder;
            folder.ShouldNotBeNullOrEmpty();
            Directory.Exists(folder).ShouldBeTrue();
        }

        [TestMethod]
        public void CaseStyle_CanGetSingleRecord()
        {
            var caseNumber = CaseNumbers.Last();
            var dto = new HarrisCaseSearchDto { CaseNumber = caseNumber.CaseNumber };
            var obj = new HarrisCriminalCaseStyle();
            IWebDriver driver = GetDriver();
            try
            {
                var result = obj.GetData(driver, dto);
                result.ShouldNotBeNull();
                result.Count.ShouldBeGreaterThan(0);
            }
            finally
            {
                driver?.Close();
                driver?.Quit();
                KillProcess("chromedriver");
            }
        }


        [TestMethod]
        public void CaseStyle_CanGet_Bulk()
        {
            var obj = new HarrisCriminalCaseStyle();
            IWebDriver driver = GetDriver();
            var result = new List<HarrisCriminalStyleDto>();
            try
            {
                result.Append(obj.GetData(driver, CaseNumbers));
                result.ShouldNotBeNull();
                result.Count.ShouldBeGreaterThan(0);
            }
            finally
            {
                driver?.Close();
                driver?.Quit();
                KillProcess("chromedriver");
            }
        }
    }
}
