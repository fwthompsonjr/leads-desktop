using Thompson.RecordSearch.Utility.Classes;

namespace legallead.search.tests.util
{
    public class BexarScriptHelperTests
    {
        [Fact]
        public void CollectionContainsScripts()
        {
            var service = BexarScriptHelper.ScriptCollection;
            Assert.NotNull(service);
            Assert.True(service.Count > 0);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("set search parameters and submit", true)]
        [InlineData("select max rows per page", true)]
        [InlineData("get list of case items", true)]
        public void CollectionContainsNamedScript(string scriptName, bool isFound)
        {
            var service = BexarScriptHelper.ScriptCollection;
            var actual = service.ContainsKey(scriptName);
            Assert.Equal(isFound, actual);
        }
    }
}
