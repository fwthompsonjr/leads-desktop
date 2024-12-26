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
        [Fact]
        public void ModelContainsDbAttributes()
        {
            var sut = HccConfigurationModel.GetModel().DbModel;
            Assert.False(string.IsNullOrEmpty(sut.Url));
            Assert.False(string.IsNullOrEmpty(sut.BeginUrl));
            Assert.False(string.IsNullOrEmpty(sut.CompleteUrl));
            Assert.False(string.IsNullOrEmpty(sut.QueryUrl));
            Assert.False(string.IsNullOrEmpty(sut.UploadUrl));
            Assert.False(string.IsNullOrEmpty(sut.HolidayUrl));

            Assert.False(string.IsNullOrEmpty(sut.UsageAppendRecordUrl));
            Assert.False(string.IsNullOrEmpty(sut.UsageCompleteRecordUrl));
            Assert.False(string.IsNullOrEmpty(sut.UsageGetLimitsUrl));
            Assert.False(string.IsNullOrEmpty(sut.UsageGetHistoryUrl));
            Assert.False(string.IsNullOrEmpty(sut.UsageGetSummaryUrl));
            Assert.False(string.IsNullOrEmpty(sut.UsageSetLimitUrl));
        }

        [Fact]
        public void ModelContainsInvoiceAttributes()
        {
            var sut = HccConfigurationModel.GetModel().InvoiceModel;
            Assert.False(string.IsNullOrEmpty(sut.Url));
            Assert.False(string.IsNullOrEmpty(sut.CompleteUrl));
            Assert.False(string.IsNullOrEmpty(sut.FetchUrl));
        }
    }
}
