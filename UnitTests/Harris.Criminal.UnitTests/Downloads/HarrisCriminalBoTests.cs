using Bogus;
using Harris.Criminal.Db.Downloads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;

namespace Harris.Criminal.Db.Tests.Downloads
{
    [TestClass]
    public class HarrisCriminalBoTests
    {

        private Faker<HarrisCriminalBo> DtoFaker;


        [TestInitialize]
        public void Setup()
        {
            if (DtoFaker == null)
            {
                var startTime = DateTime.Now.AddYears(-5);
                var endTime = DateTime.Now.AddYears(5);
                var fmt = "yyyyMMdd";
                DtoFaker = new Faker<HarrisCriminalBo>()
                    .RuleFor(f => f.Index, r => r.IndexFaker)
                    .RuleFor(f => f.DateDatasetProduced, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.CourtDivisionIndicator, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.CaseNumber, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.FilingDate, r => r.Date.Between(startTime, endTime).ToString(fmt))
                    .RuleFor(f => f.InstrumentType, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.CaseDisposition, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.Court, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.CaseStatus, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DefendantStatus, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.BondAmount, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.CurrentOffense, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.CurrentOffenseLiteral, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.CurrentLevelAndDegree, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.NextAppearanceDate, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DocketCalendarNameCode, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.CalendarReason, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DefendantName, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DefendantSPN, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DefendantRace, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DefendantSex, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DefendantDateOfBirth, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DefendantStreetNumber, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DefendantStreetName, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DefendantCity, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DefendantState, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DefendantZip, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.AttorneyName, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.AttorneySPN, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.AttorneyConnectionCode, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.AttorneyConnectionLiteral, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DefendantPlaceOfBirth, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.DefUSCitizenFlag, r => r.Random.AlphaNumeric(15));
            }
        }

        [TestMethod]
        public void DateFiled_Check()
        {
            var startTime = DateTime.Now.AddYears(-5).AddHours(-1);
            var endTime = DateTime.Now.AddYears(5);
            var dataset = DtoFaker.Generate(15);
            dataset.ForEach(d => d.DateFiled.ShouldBeGreaterThan(startTime));
        }

        [TestMethod]
        public void Indexer_Get()
        {
            var obj = DtoFaker.Generate();

            obj.Index.ToString().ShouldBe(obj[0]);
            obj.DateDatasetProduced.ShouldBe(obj[1]);
            obj.CourtDivisionIndicator.ShouldBe(obj[2]);
            obj.CaseNumber.ShouldBe(obj[3]);
            obj.FilingDate.ShouldBe(obj[4]);
            obj.InstrumentType.ShouldBe(obj[5]);
            obj.CaseDisposition.ShouldBe(obj[6]);
            obj.Court.ShouldBe(obj[7]);
            obj.CaseStatus.ShouldBe(obj[8]);
            obj.DefendantStatus.ShouldBe(obj[9]);
            obj.BondAmount.ShouldBe(obj[10]);
            obj.CurrentOffense.ShouldBe(obj[11]);
            obj.CurrentOffenseLiteral.ShouldBe(obj[12]);
            obj.CurrentLevelAndDegree.ShouldBe(obj[13]);
            obj.NextAppearanceDate.ShouldBe(obj[14]);
            obj.DocketCalendarNameCode.ShouldBe(obj[15]);
            obj.CalendarReason.ShouldBe(obj[16]);
            obj.DefendantName.ShouldBe(obj[17]);
            obj.DefendantSPN.ShouldBe(obj[18]);
            obj.DefendantRace.ShouldBe(obj[19]);
            obj.DefendantSex.ShouldBe(obj[20]);
            obj.DefendantDateOfBirth.ShouldBe(obj[21]);
            obj.DefendantStreetNumber.ShouldBe(obj[22]);
            obj.DefendantStreetName.ShouldBe(obj[23]);
            obj.DefendantCity.ShouldBe(obj[24]);
            obj.DefendantState.ShouldBe(obj[25]);
            obj.DefendantZip.ShouldBe(obj[26]);
            obj.AttorneyName.ShouldBe(obj[27]);
            obj.AttorneySPN.ShouldBe(obj[28]);
            obj.AttorneyConnectionCode.ShouldBe(obj[29]);
            obj.AttorneyConnectionLiteral.ShouldBe(obj[30]);
            obj.DefendantPlaceOfBirth.ShouldBe(obj[31]);
            obj.DefUSCitizenFlag.ShouldBe(obj[32]);

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
            for (int i = 0; i < HarrisCriminalDto.FieldNames.Count; i++)
            {
                obj[i].ShouldNotBe(src[i]);
                obj[i] = src[i];
            }


            obj.Index.ToString().ShouldBe(src[0]);
            obj.DateDatasetProduced.ShouldBe(src[1]);
            obj.CourtDivisionIndicator.ShouldBe(src[2]);
            obj.CaseNumber.ShouldBe(src[3]);
            obj.FilingDate.ShouldBe(src[4]);
            obj.InstrumentType.ShouldBe(src[5]);
            obj.CaseDisposition.ShouldBe(src[6]);
            obj.Court.ShouldBe(src[7]);
            obj.CaseStatus.ShouldBe(src[8]);
            obj.DefendantStatus.ShouldBe(src[9]);
            obj.BondAmount.ShouldBe(src[10]);
            obj.CurrentOffense.ShouldBe(src[11]);
            obj.CurrentOffenseLiteral.ShouldBe(src[12]);
            obj.CurrentLevelAndDegree.ShouldBe(src[13]);
            obj.NextAppearanceDate.ShouldBe(src[14]);
            obj.DocketCalendarNameCode.ShouldBe(src[15]);
            obj.CalendarReason.ShouldBe(src[16]);
            obj.DefendantName.ShouldBe(src[17]);
            obj.DefendantSPN.ShouldBe(src[18]);
            obj.DefendantRace.ShouldBe(src[19]);
            obj.DefendantSex.ShouldBe(src[20]);
            obj.DefendantDateOfBirth.ShouldBe(src[21]);
            obj.DefendantStreetNumber.ShouldBe(src[22]);
            obj.DefendantStreetName.ShouldBe(src[23]);
            obj.DefendantCity.ShouldBe(src[24]);
            obj.DefendantState.ShouldBe(src[25]);
            obj.DefendantZip.ShouldBe(src[26]);
            obj.AttorneyName.ShouldBe(src[27]);
            obj.AttorneySPN.ShouldBe(src[28]);
            obj.AttorneyConnectionCode.ShouldBe(src[29]);
            obj.AttorneyConnectionLiteral.ShouldBe(src[30]);
            obj.DefendantPlaceOfBirth.ShouldBe(src[31]);
            obj.DefUSCitizenFlag.ShouldBe(src[32]);

            // attempt to set out of range field indexes
            for (int i = 33; i < 50; i++)
            {
                obj[i] = src[i - 30];
            }
        }
    }
}
