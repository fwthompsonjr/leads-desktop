using Bogus;
using LegalLead.PublicData.Search.Common;

namespace legallead.search.tests.common
{
    public class UserPasswordChangeModelTests
    {
        [Fact]
        public void ModelCanBeCreated()
        {
            var error = Record.Exception(() => { _ = faker.Generate(); });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void ModelCanGetByIndex(int position)
        {
            var sut = faker.Generate();
            var expected = position == 4;
            var actual = string.IsNullOrEmpty(sut[position]);
            Assert.Equal(expected, actual);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void ModelCanSetByIndex(int position)
        {
            var error = Record.Exception(() =>
            {
                var sut = faker.Generate();
                var temp = faker.Generate()[position];
                sut[position] = temp;
            });
            Assert.Null(error);
        }


        private static readonly Faker<UserPasswordChangeModel> faker
            = new Faker<UserPasswordChangeModel>()
            .RuleFor(x => x.UserName, y => y.Person.Email)
            .RuleFor(x => x.OldPassword, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.NewPassword, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.ConfirmPassword, y => y.Random.AlphaNumeric(25));
    }
}
