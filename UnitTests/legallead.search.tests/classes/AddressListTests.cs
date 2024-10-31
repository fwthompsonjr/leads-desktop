using LegalLead.PublicData.Search.Util;
using System;
using Thompson.RecordSearch.Utility.Db;

namespace legallead.search.tests.classes
{

    public class AddressListTests
    {
        [Fact]
        public void DtoCanGetDallasList()
        {
            var dropdown = AddressListDto.DallasList;
            Assert.NotNull(dropdown);
        }

        [Theory]
        [InlineData("county")]
        [InlineData("criminal")]
        [InlineData("district")]
        [InlineData("justice")]
        public void DtoCanGetCollection(string name)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var list = AddressListDto.DallasList;
            var collection = list.Find(x => x.Name.Equals(name, oic));
            Assert.NotNull(collection);
            Assert.NotEmpty(collection.Items);
        }


        [Theory]
        [InlineData("county")]
        [InlineData("district")]
        [InlineData("justice")]
        public void DtoCanGetBexarCollection(string name)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var list = AddressListDto.BexarList;
            var collection = list.Find(x => x.Name.Equals(name, oic));
            Assert.NotNull(collection);
            Assert.NotEmpty(collection.Items);
        }


        [Theory]
        [InlineData("missing", "County Court at Law No. 1", false)]
        [InlineData("county", "County Court at Law No. 1", true)]
        [InlineData("county", "County Court at Law No. 2", true)]
        [InlineData("county", "County Court at Law No. 3", true)]
        [InlineData("county", "County Court at Law No. 4", true)]
        [InlineData("county", "County Court at Law No. 5", true)]
        public void DtoCanGetCollectionAddress(string name, string court, bool expected)
        {
            var actual = DallasCourtLookupService.GetAddress(name, court);
            if (expected) Assert.NotNull(actual);
            else Assert.Null(actual);
        }
    }
}