using Bogus;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace legallead.search.tests.classes
{
    public class ExcelWriterTests
    {
        [Theory]
        [InlineData(70)]
        public void WriterCanGeneratePackage(int webId)
        {
            var error = Record.Exception(() =>
            {
                var people = PersonAddressFaker.Generate(20);
                var writer = new ExcelWriter();
                _ = writer.ConvertToPersonTable(addressList: people, worksheetName: "addresses", websiteId: webId);
            });
            Assert.Null(error);
        }

        private static readonly Faker<PersonAddress> PersonAddressFaker
            = new Faker<PersonAddress>()
            .RuleFor(x => x.Name, y =>
            {
                return $"{y.Person.LastName}, {y.Person.FirstName}";
            })
            .RuleFor(x => x.Zip, y => y.Address.ZipCode())
            .RuleFor(x => x.Address1, y => y.Address.StreetAddress())
            .RuleFor(x => x.Address3, y =>
            {
                return $"{y.Address.City}, {y.Address.StateAbbr()} {y.Address.ZipCode()}";
            })
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.Court, y => "Precinct 2")
            .RuleFor(x => x.Plantiff, y => y.Company.CompanyName())
            .RuleFor(x => x.CaseType, y => y.Random.AlphaNumeric(8))
            .FinishWith((a, b) =>
            {
                b.CaseStyle = $"{b.Name} vs. {b.Plantiff}";
            });
    }
}
