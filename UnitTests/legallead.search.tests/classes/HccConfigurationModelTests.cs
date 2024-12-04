using Thompson.RecordSearch.Utility.Dto;

namespace legallead.search.tests.classes
{
    public class HccConfigurationModelTests
    {
        [Fact]
        public void ModelCanBeCreated()
        {
            var actual = HccConfigurationModel.GetModel();
            Assert.NotNull(actual);
        }

        [Fact]
        public void ModelContainsWebsiteAttributes()
        {
            var sut = HccConfigurationModel.GetModel();
            Assert.False(string.IsNullOrEmpty(sut.Url));
            Assert.False(string.IsNullOrEmpty(sut.Settings));
            Assert.False(string.IsNullOrEmpty(sut.Monthly));
        }

        [Fact]
        public void ModelContainsRemoteAttributes()
        {
            var sut = HccConfigurationModel.GetModel().RemoteModel;
            Assert.False(string.IsNullOrEmpty(sut.Url));
            Assert.False(string.IsNullOrEmpty(sut.PostUrl));
            Assert.False(string.IsNullOrEmpty(sut.FetchUrl));
        }
    }
}
