using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Tests
{
    [TestClass]
    public class HarrisCriminalStyleDtoTests
    {
        private Faker<HarrisCriminalStyleDto> DtoFaker;

        [TestInitialize]
        public void Setup()
        {
            if (DtoFaker == null)
            {
                var startTime = DateTime.Now.AddYears(-5);
                var endTime = DateTime.Now.AddYears(5);
                var fmt = "m/d/yyyy";
                DtoFaker = new Faker<HarrisCriminalStyleDto>()
                    .RuleFor(f => f.Index, r => r.IndexFaker)
                    .RuleFor(f => f.CaseNumber, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.Style, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.FileDate, r => r.Date.Between(startTime, endTime).ToString(fmt))
                    .RuleFor(f => f.Court, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.Status, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.TypeOfActionOrOffense, r => r.Random.AlphaNumeric(15));
            }
        }

        [TestMethod]
        public void CanConstruct()
        {
            var obj = new HarrisCriminalStyleDto();
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void CanInit()
        {
            var obj = DtoFaker.Generate();
            Assert.IsNotNull(obj);
        }
        [TestMethod]
        public void HasFields()
        {
            var fields = HarrisCriminalStyleDto.FieldNames;
            Assert.AreEqual(7, fields.Count);
        }

        [TestMethod]
        public void Indexer_Get()
        {
            var obj = DtoFaker.Generate();

            obj.Index.ToString().ShouldBe(obj[0]);
            obj.CaseNumber.ShouldBe(obj[1]);
            obj.Style.ShouldBe(obj[2]);
            obj.FileDate.ShouldBe(obj[3]);
            obj.Court.ShouldBe(obj[4]);
            obj.Status.ShouldBe(obj[5]);
            obj.TypeOfActionOrOffense.ShouldBe(obj[6]);

            for (int i = 33; i < 50; i++)
            {
                obj[i].ShouldBeNull();
            }
        }

        [TestMethod]
        public void Indexer_Set()
        {
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            for (int i = 0; i < HarrisCriminalStyleDto.FieldNames.Count; i++)
            {
                obj[i].ShouldNotBe(src[i]);
                obj[i] = src[i];
            }


            obj.Index.ToString().ShouldBe(src[0]);
            obj.CaseNumber.ShouldBe(src[1]);
            obj.Style.ShouldBe(src[2]);
            obj.FileDate.ShouldBe(src[3]);
            obj.Court.ShouldBe(src[4]);
            obj.Status.ShouldBe(src[5]);
            obj.TypeOfActionOrOffense.ShouldBe(src[6]);

            // attempt to set out of range field indexes
            for (int i = 33; i < 50; i++)
            {
                obj[i] = src[i - 30];
            }
        }
    }
}
