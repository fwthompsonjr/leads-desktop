using Bogus;
using Harris.Criminal.Db.Downloads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Linq;

namespace Harris.Criminal.Db.Tests.Downloads
{
    [TestClass]
    public class HarrisCountyListDtoTests
    {
        private Faker<HarrisCountyListDto> ListFaker;
        private Faker<HarrisCriminalDto> DtoFaker;
        private bool isInitialized = false;
        [TestInitialize]
        public void Setup()
        {
            if (!isInitialized)
            {
                Startup.Read();
            }
            if (DtoFaker == null)
            {
                var startTime = DateTime.Now.AddYears(-5);
                var endTime = DateTime.Now.AddYears(5);
                var fmt = "yyyyMMdd";
                DtoFaker = new Faker<HarrisCriminalDto>()
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
            if (ListFaker == null)
            {
                ListFaker = new Faker<HarrisCountyListDto>()
                    .RuleFor(f => f.Name, r => r.Database.Column())
                    .RuleFor(f => f.CreateDate, r => r.Date.Soon())
                    .RuleFor(f => f.FileDate, r => r.Date.Soon())
                    .RuleFor(f => f.MaxFilingDate, r => r.Date.Recent())
                    .RuleFor(f => f.MinFilingDate, r => r.Date.Recent())
                    .RuleFor(f => f.Data, r =>
                    {
                        var nbr = r.Random.Int(1, 10);
                        if (nbr == 1) return null;
                        return DtoFaker.Generate(nbr);
                    });
            }
        }

        [TestMethod]
        public void CanInit()
        {
            var item = ListFaker.Generate(4);
            item.ShouldNotBeNull();
        }

        [TestMethod]
        public void Downloads_HasFileNames()
        {
            var filenames = Startup.Downloads.FileNames;
            filenames.ShouldNotBeNull();
            filenames.Count.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        public void Downloads_HasFileData()
        {
            var filenames = Startup.Downloads.FileNames;
            var data = Startup.Downloads.DataList;
            filenames.ShouldNotBeNull();
            filenames.Count.ShouldBeGreaterThan(0);
            data.ShouldNotBeNull();
            data.Count.ShouldBeGreaterThan(0);
            data.Count.ShouldBe(filenames.Count);
        }


        [TestMethod]
        public void Downloads_FileData_HasReasonable_MinDate()
        {
            DateTime reasonableDate = DateTime.Now.AddYears(-5);
            var filenames = Startup.Downloads.FileNames;
            var data = Startup.Downloads.DataList;
            var minDate = data.Select(x => x.MinFilingDate).ToList();
            minDate.ForEach(mn => mn.ShouldBeGreaterThan(reasonableDate));
        }

        [TestMethod]
        public void Downloads_FileData_HasReasonable_MaxDate()
        {
            DateTime reasonableDate = DateTime.Now.AddMonths(2);
            var filenames = Startup.Downloads.FileNames;
            var data = Startup.Downloads.DataList;
            var minDate = data.Select(x => x.MinFilingDate).ToList();
            minDate.ForEach(mn => mn.ShouldBeLessThan(reasonableDate));
        }

    }
}
