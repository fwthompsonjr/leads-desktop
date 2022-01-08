using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Globalization;
using Thompson.RecordSearch.Utility.Parsing;

namespace Thompson.RecordSearch.Utility.Tests.Data
{
    [TestClass]
    public class CaseStyleDbParserTests
    {

        private class SytleData
        {
            public string Header { get; set; }
            public string Separator { get; set; }
            public string Person { get; set; }
            public string Spn { get; set; }
            public string Dob { get; set; }

            public string CaseStyle => $"{Header} {Separator} {Person}";

            public override string ToString()
            {
                var resp = $"{Header} {Separator} {Person} (SPN: {Spn}) (DOB: {Dob})";
                if (string.IsNullOrEmpty(resp))
                {
                    return base.ToString();
                }

                return resp;
            }
        }

        private Faker<SytleData> FakeData;
        private CaseStyleDbParser Parser;

        [TestInitialize]
        public void Setup()
        {
            if (FakeData == null)
            {
                var dobStart = DateTime.Now.AddYears(-80);
                var dobEnd = DateTime.Now.AddYears(-15);
                FakeData = new Faker<SytleData>()
                    .RuleFor(f => f.Header, r => $"The State of {r.Address.State()}")
                    .RuleFor(f => f.Separator, r => "vs.")
                    .RuleFor(f => f.Person, r => string.Concat(r.Person.LastName, ", ", r.Person.FirstName))
                    .RuleFor(f => f.Spn, r => r.Random.Int(100000, 9999999).ToString("d8", CultureInfo.CurrentCulture))
                    .RuleFor(f => f.Dob, r => r.Date.Between(dobStart, dobEnd).ToString("MM/dd/yyyy", CultureInfo.CurrentCulture));
            }
            var data = FakeData.Generate();
            Parser = new CaseStyleDbParser { Data = data.ToString() };
        }

        [TestMethod]
        public void CanParse_WithVs()
        {
            Parser.CanParse().ShouldBeTrue();
        }

        [TestMethod]
        public void CanParse_WithoutData_IsFalse()
        {
            Parser.Data = string.Empty;
            Parser.CanParse().ShouldBeFalse();
        }

        [TestMethod]
        public void CanParse_WithoutVs_IsFalse()
        {
            var dto = FakeData.Generate();
            dto.Separator = "versus";
            Parser.Data = dto.ToString();
            Parser.CanParse().ShouldBeFalse();
        }

        [TestMethod]
        public void Parser_CanExtract_Defendant()
        {
            var dto = FakeData.Generate(10);
            foreach (var item in dto)
            {
                Parser.Data = item.ToString();
                var expected = item.Person;
                var response = Parser.Parse();
                response.Defendant.ShouldBe(expected);
            }
        }

        [TestMethod]
        public void Parser_CanExtract_Defendant_Once()
        {
            var item = FakeData.Generate();
            Parser.Data = item.ToString();
            var expected = item.Person;
            var response = Parser.Parse();
            response.Defendant.ShouldBe(expected);
        }

        [TestMethod]
        public void Parser_CanExtract_CaseStyle()
        {
            var dto = FakeData.Generate(10);
            foreach (var item in dto)
            {
                Parser.Data = item.ToString();
                var expected = item.CaseStyle;
                var response = Parser.Parse();
                response.CaseData.ShouldBe(expected);
            }
        }

        [TestMethod]
        public void Parser_CanExtract_CaseStyle_Once()
        {
            var item = FakeData.Generate();
            Parser.Data = item.ToString();
            var expected = item.CaseStyle;
            var response = Parser.Parse();
            response.CaseData.ShouldBe(expected);
        }

        [TestMethod]
        public void Parser_CanExtract_Plantiff()
        {
            var dto = FakeData.Generate(10);
            foreach (var item in dto)
            {
                Parser.Data = item.ToString();
                var expected = item.Header;
                var response = Parser.Parse();
                response.Plantiff.ShouldBe(expected);
            }
        }

        [TestMethod]
        public void Parser_CanExtract_Plantiff_Once()
        {
            var item = FakeData.Generate();
            Parser.Data = item.ToString();
            var expected = item.Header;
            var response = Parser.Parse();
            response.Plantiff.ShouldBe(expected);
        }
    }
}
