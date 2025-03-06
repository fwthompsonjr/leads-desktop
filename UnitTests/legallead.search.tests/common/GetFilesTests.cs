
using LegalLead.PublicData.Search.Classes;

namespace legallead.search.tests.common
{
    public class GetFilesTests
    {
        [Fact]
        public void ListCanBeCreated()
        {
            var error = Record.Exception(() => { _ = CommonFolderHelper.GetFiles(); });
            Assert.Null(error);
        }
    }
}