using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.UnitTests.Models
{
    [TestClass]
    public class CaseDataAddressTests
    {
        private Faker<CaseDataAddress> DtoFaker;

        [TestInitialize]
        public void Setup()
        {
            if (DtoFaker == null)
            {
                DtoFaker = new Faker<CaseDataAddress>()
                    .RuleFor(f => f.Case, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.Role, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.Party, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.Attorney, r => r.Random.AlphaNumeric(15));
            }
        }

        [TestMethod]
        public void CanConstruct()
        {
            var obj = new CaseDataAddress();
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void CanInit()
        {
            var obj = DtoFaker.Generate();
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void CanSet_Case()
        {
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            obj.Case = src.Case;
            obj.Case.ShouldBe(src.Case);
        }

        [TestMethod]
        public void CanSet_Role()
        {
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            obj.Role = src.Role;
            obj.Role.ShouldBe(src.Role);
        }

        [TestMethod]
        public void CanSet_Party()
        {
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            obj.Party = src.Party;
            obj.Party.ShouldBe(src.Party);
        }

        [TestMethod]
        public void CanSet_Attorney()
        {
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            obj.Attorney = src.Attorney;
            obj.Attorney.ShouldBe(src.Attorney);
        }
    }
}
