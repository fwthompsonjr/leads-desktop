using Thompson.RecordSearch.Utility.Classes;

namespace legallead.search.tests.util
{
    public class HidalgoScriptHelperTests
    {
        [Fact]
        public void CollectionContainsScripts()
        {
            var service = HidalgoScriptHelper.ScriptCollection;
            Assert.NotNull(service);
            Assert.True(service.Count > 0);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("say hello", true)]
        [InlineData("nav to search screen", true)]
        [InlineData("populate search parameters", true)]
        [InlineData("find no match", true)]
        public void CollectionContainsNamedScript(string scriptName, bool isFound)
        {
            var service = HidalgoScriptHelper.ScriptCollection;
            var actual = service.ContainsKey(scriptName);
            Assert.Equal(isFound, actual);
        }
        [Theory]
        [InlineData("{0}", true)]
        [InlineData("{1}", true)]
        [InlineData("{2}", true)]
        [InlineData("var bttn = document.getElementById(btnid);", true)]
        [InlineData("bttn.click();", true)]
        [InlineData("{3}", false)]
        public void ParameterScriptContainsToken(string token, bool expected)
        {
            const string scriptName = "populate search parameters";
            var service = HidalgoScriptHelper.ScriptCollection;
            var script = service[scriptName];
            Assert.False(string.IsNullOrEmpty(script));
            var actual = script.Contains(token);
            Assert.Equal(expected, actual);
        }
        [Theory]
        [InlineData("say hello")]
        public void ScriptCanBeParsed(string scriptName)
        {
            var error = Record.Exception(() =>
            {
                var service = HidalgoScriptHelper.ScriptCollection;
                var js = service[scriptName];
                JsValidate.IsValid(js);
            });
            Assert.Null(error);
        }
    }
}