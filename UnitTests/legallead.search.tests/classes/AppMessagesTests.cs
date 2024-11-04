using Thompson.RecordSearch.Utility.Classes;

namespace legallead.search.tests.classes
{
    public class AppMessagesTests
    {
        [Theory]
        [InlineData("Missing", true)]
        [InlineData("STATUS_MSG_SEARCH_FOUND_RECORDS", false)]
        public void AppMessageContainsKey(string message, bool expected)
        {
            var text = AppMessages.GetMessage(message);
            var actual = string.IsNullOrEmpty(text);
            Assert.Equal(expected, actual);
        }
    }
}
