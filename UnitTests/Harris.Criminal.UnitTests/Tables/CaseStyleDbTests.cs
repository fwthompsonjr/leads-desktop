using Bogus;
using Harris.Criminal.Db.Tables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Globalization;

namespace Harris.Criminal.Db.Tests.Tables
{
    [TestClass]
    public class CaseStyleDbTests
    {
        private Faker<CaseStyleDb> DtoFaker;

        [TestInitialize]
        public void Setup()
        {
            if (DtoFaker == null)
            {
                var startTime = DateTime.Now.AddYears(-5);
                var endTime = DateTime.Now.AddYears(5);
                var fmt = "m/d/yyyy";
                DtoFaker = new Faker<CaseStyleDb>()
                    .RuleFor(f => f.CaseNumber, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.Style, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.FileDate, r => r.Date.Between(startTime, endTime).ToString(fmt, CultureInfo.CurrentCulture))
                    .RuleFor(f => f.Court, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.Status, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.TypeOfActionOrOffense, r => r.Random.AlphaNumeric(15));
            }
        }

        [TestMethod]
        public void CanConstruct()
        {
            var obj = new CaseStyleDb();
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
            var fields = CaseStyleDb.FieldNames;
            Assert.AreEqual(6, fields.Count);
        }

        [TestMethod]
        public void Indexer_Get()
        {
            var obj = DtoFaker.Generate();

            obj.CaseNumber.ShouldBe(obj[0]);
            obj.Style.ShouldBe(obj[1]);
            obj.FileDate.ShouldBe(obj[2]);
            obj.Court.ShouldBe(obj[3]);
            obj.Status.ShouldBe(obj[4]);
            obj.TypeOfActionOrOffense.ShouldBe(obj[5]);

            for (int i = 33; i < 50; i++)
            {
                obj[i].ShouldBeNull();
            }
        }

        [TestMethod]
        public void Indexer_Get_With_FieldNames()
        {
            var obj = DtoFaker.Generate();
            var names = CaseStyleDb.FieldNames;

            obj.CaseNumber.ShouldBe(obj[names[0]]);
            obj.Style.ShouldBe(obj[names[1]]);
            obj.FileDate.ShouldBe(obj[names[2]]);
            obj.Court.ShouldBe(obj[names[3]]);
            obj.Status.ShouldBe(obj[names[4]]);
            obj.TypeOfActionOrOffense.ShouldBe(obj[names[5]]);

            obj[""].ShouldBeNull();
            obj["abcdefg"].ShouldBeNull();
        }

        [TestMethod]
        public void Indexer_Get_Negative()
        {
            var obj = DtoFaker.Generate();
            obj[-1].ShouldBeNull();
        }

        [TestMethod]
        public void Indexer_Set()
        {
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            for (int i = 0; i < CaseStyleDb.FieldNames.Count; i++)
            {
                obj[i].ShouldNotBe(src[i]);
                obj[i] = src[i];
            }

            obj.CaseNumber.ShouldBe(src[0]);
            obj.Style.ShouldBe(src[1]);
            obj.FileDate.ShouldBe(src[2]);
            obj.Court.ShouldBe(src[3]);
            obj.Status.ShouldBe(src[4]);
            obj.TypeOfActionOrOffense.ShouldBe(src[5]);

            // attempt to set out of range field indexes
            for (int i = 33; i < 50; i++)
            {
                obj[i] = src[i - 30];
            }
        }

        [TestMethod]
        public void Indexer_Set_With_FieldNames()
        {
            var list = DtoFaker.Generate(2);
            var names = CaseStyleDb.FieldNames;
            var obj = list[0];
            var src = list[1];
            for (int i = 0; i < names.Count; i++)
            {
                obj[names[i]].ShouldNotBe(src[names[i]]);
                obj[names[i]] = src[names[i]];
            }

            obj.CaseNumber.ShouldBe(src[names[0]]);
            obj.Style.ShouldBe(src[names[1]]);
            obj.FileDate.ShouldBe(src[names[2]]);
            obj.Court.ShouldBe(src[names[3]]);
            obj.Status.ShouldBe(src[names[4]]);
            obj.TypeOfActionOrOffense.ShouldBe(src[names[5]]);

            // attempt to set out of range field indexes
            obj[""] = src[""];
            obj["abcdefg"] = src["abcdefg"];
        }
    }
}
