﻿using Harris.Criminal.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Shouldly;
using System;
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
        private readonly int MxCaseNumbers = 10;
        private List<HarrisCaseSearchDto> CaseNumbers;
        private string MaximumFileDate;
        private string MinimumFileDate;
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
                var list = dtos.GroupBy(x => x.UniqueIndex()).Select(x => x.FirstOrDefault());
                CaseNumbers = list.Take(MxCaseNumbers).ToList();
                MaximumFileDate = list.Max(x => x.DateFiled);
                MinimumFileDate = list.Min(x => x.DateFiled);
            }
        }

        [TestMethod]
        public void Download_HasACorrectTarget()
        {
            var folder = HarrisCriminalCaseStyle.DownloadFolder;
            folder.ShouldNotBeNullOrEmpty();
            Directory.Exists(folder).ShouldBeTrue();
        }

        [TestMethod]
        [TestCategory("Integration Only")]
        public void CaseStyle_CanGetSingleRecord()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Assert.Inconclusive("This method to be executed in debug mode only.");
            }
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
        [TestCategory("Integration Only")]
        public void CaseStyle_CanGet_Bulk()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Assert.Inconclusive("This method to be executed in debug mode only.");
            }
            const string fmt = "yyyyMMdd";
            DateTime dateBase = DateTime.MaxValue;
            var dtmin = MaximumFileDate.ToExactDate(fmt, dateBase).AddDays(-10);
            var dtmax = MaximumFileDate.ToExactDate(fmt, dateBase);
            dtmin.ShouldNotBe(dateBase);
            var dateRange = Convert.ToInt32(dtmax.Subtract(dtmin).TotalDays) + 1;

            var obj = new HarrisCriminalCaseStyle();
            IWebDriver driver = GetDriver();
            var result = new List<HarrisCriminalStyleDto>();
            try
            {
                result.Append(obj.GetCases(driver, dtmax, dateRange));
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
        public void CanParse_Min_Max()
        {
            const string fmt = "yyyyMMdd";
            DateTime dateBase = DateTime.MaxValue;
            var dtmin = MinimumFileDate.ToExactDate(fmt, dateBase);
            var dtmax = MaximumFileDate.ToExactDate(fmt, dateBase);
            dtmin.ShouldNotBe(dateBase);
            dtmax.ShouldNotBe(dateBase);
            dtmin.ShouldBeLessThanOrEqualTo(dtmax);
        }

    }
}