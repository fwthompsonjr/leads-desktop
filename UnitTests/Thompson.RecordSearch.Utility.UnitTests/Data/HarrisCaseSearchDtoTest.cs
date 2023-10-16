using Bogus;
using Harris.Criminal.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Globalization;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Tests.Data
{
    [TestClass]
    public class HarrisCaseSearchDtoTest
    {
        private Faker<HarrisCaseSearchDto> DtoFaker;
        const string datefmt = "MM/dd/yyyy";
        [TestInitialize]
        public void Setup()
        {
            if (DtoFaker == null)
            {
                var startTime = DateTime.Now.AddYears(-5);
                var endTime = DateTime.Now.AddYears(5);
                DtoFaker = new Faker<HarrisCaseSearchDto>()
                    .RuleFor(f => f.CaseNumber, r => r.Random.Long(120000000000, 190000000000).ToString("d", CultureInfo.CurrentCulture))
                    .RuleFor(f => f.DateFiled, r => r.Date.Between(startTime, endTime).ToString(datefmt, CultureInfo.CurrentCulture))
                    .RuleFor(f => f.Court, r => r.Random.AlphaNumeric(3))
                    .RuleFor(f => f.DateFormat, r => datefmt);
            }
        }

        [TestMethod]
        public void CanConstruct()
        {
            var obj = new HarrisCaseSearchDto();
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void CanInit()
        {
            var obj = DtoFaker.Generate();
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void CanGenerate_UniqueIndex()
        {
            var obj = DtoFaker.Generate();
            var fileDate = obj.DateFiled.ToExactDate(datefmt, DateTime.MaxValue);
            var expected = $"{fileDate:s}~{obj.CaseNumber}~{obj.Court}";
            var actual = obj.UniqueIndex();
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void CanSet_CaseNumber()
        {
            var obj = DtoFaker.Generate(2);
            var expected = obj[1].CaseNumber;
            obj[0].CaseNumber = expected;
            obj[0].CaseNumber.ShouldBe(expected);
        }

        [TestMethod]
        public void CanSet_DateFiled()
        {
            var obj = DtoFaker.Generate(2);
            var expected = obj[1].DateFiled;
            obj[0].DateFiled = expected;
            obj[0].DateFiled.ShouldBe(expected);
        }

        [TestMethod]
        public void CanSet_Court()
        {
            var obj = DtoFaker.Generate(2);
            var expected = obj[1].Court;
            obj[0].Court = expected;
            obj[0].Court.ShouldBe(expected);
        }

        [TestMethod]
        public void CanSet_DateFormat()
        {
            var obj = DtoFaker.Generate(2);
            var expected = obj[1].DateFormat;
            obj[0].DateFormat = expected;
            obj[0].DateFormat.ShouldBe(expected);
        }
    }
}
