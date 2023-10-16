using Bogus;
using Harris.Criminal.Db.Tables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Harris.Criminal.Db.Tests.Tables
{
    [TestClass]
    public class ReferenceTableTests
    {
        private Faker<ReferenceDatum> DatumFaker;
        private Faker<ReferenceTable> TableFaker;
        private bool isInitialized = false;

        [TestInitialize]
        public void Setup()
        {
            if (!isInitialized)
            {
                Startup.Read();
                isInitialized = true;
            }
            if (DatumFaker == null)
            {
                DatumFaker = new Faker<ReferenceDatum>()
                    .RuleFor(f => f.Code, r => r.Address.StateAbbr())
                    .RuleFor(f => f.Literal, r => r.Address.State());
            }
            if (TableFaker == null)
            {
                TableFaker = new Faker<ReferenceTable>()
                    .RuleFor(f => f.Name, r => r.Company.CompanyName())
                    .RuleFor(f => f.Data, r =>
                    {
                        var nbr = r.Random.Int(-2, 20);
                        if (nbr < 0)
                        {
                            return null;
                        }

                        return DatumFaker.Generate(nbr);
                    });
            }
        }

        [TestMethod]
        public void CanConstruct()
        {
            var obj = new ReferenceTable();
            obj.ShouldNotBeNull();
        }

        [TestMethod]
        public void CanInit()
        {
            var obj = TableFaker.Generate(5);
            obj.ShouldNotBeNull();
        }

        [TestMethod]
        public void References_HasFileNames()
        {
            const int expected = 10;
            var filenames = Startup.References.FileNames;
            filenames.ShouldNotBeNull();
            filenames.Count.ShouldBe(expected);
        }

        [TestMethod]
        public void References_HasDataLists()
        {
            const int expected = 10;
            var data = Startup.References.DataList;
            data.ShouldNotBeNull();
            data.Count.ShouldBe(expected);
        }
    }
}
