using Bogus;
using Harris.Criminal.Db.Tables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Harris.Criminal.Db.Tests.Tables
{
    [TestClass]
    public class ReferenceDatumTests
    {
        private Faker<ReferenceDatum> DatumFaker;

        [TestInitialize]
        public void Setup()
        {
            if (DatumFaker == null)
            {
                DatumFaker = new Faker<ReferenceDatum>()
                    .RuleFor(f => f.Code, r => r.Address.StateAbbr())
                    .RuleFor(f => f.Literal, r => r.Address.State());
            }
        }

        [TestMethod]
        public void CanInit()
        {
            var obj = new ReferenceDatum();
            obj.ShouldNotBeNull();
        }

        [TestMethod]
        public void CanRead_Code()
        {
            var obj = DatumFaker.Generate();
            var test = obj.Code;
            obj.Code.ShouldNotBeNullOrWhiteSpace();
            obj.Code.ShouldBe(test);
        }

        [TestMethod]
        public void CanRead_Literal()
        {
            var obj = DatumFaker.Generate();
            var test = obj.Literal;
            obj.Literal.ShouldNotBeNullOrWhiteSpace();
            obj.Literal.ShouldBe(test);
        }

        [TestMethod]
        public void CanWrite_Code()
        {
            var list = DatumFaker.Generate(5);
            var obj = list[1];
            var src = list[4];
            var test = src.Code + "-changed";
            obj.Code = test;
            obj.Code.ShouldBe(test);
        }

        [TestMethod]
        public void CanWrite_Literal()
        {
            var list = DatumFaker.Generate(5);
            var obj = list[1];
            var src = list[4];
            var test = src.Literal + "-changed";
            obj.Literal = test;
            obj.Literal.ShouldBe(test);
        }
    }
}
