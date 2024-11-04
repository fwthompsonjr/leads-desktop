using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Classes;

namespace legallead.search.tests.classes
{
    public class WilliamsonScriptHelperTests
    {
        [Fact]
        public void CollectionContainsScripts()
        {
            var service = collection;
            Assert.NotNull(service);
            Assert.True(service.Count > 0);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("say hello", true)]
        [InlineData("get navigation url", true)]
        [InlineData("nav to search screen", true)]
        [InlineData("populate search parameters", true)]
        [InlineData("find no match", true)]
        [InlineData("get person address", true)]
        [InlineData("find case detail links", true)]
        [InlineData("click case detail links", true)]
        public void CollectionContainsNamedScript(string scriptName, bool isFound)
        {
            var service = collection;
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
            var service = collection;
            var script = service[scriptName];
            Assert.False(string.IsNullOrEmpty(script));
            var actual = script.Contains(token);
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void CollectionContainsNavigationUrl()
        {
            const char star = '*';
            const string scriptName = "get navigation url";
            var service = collection;
            var isFound = service.TryGetValue(scriptName, out var actual);
            Assert.True(isFound);
            Assert.False(string.IsNullOrEmpty(actual));
            Assert.Contains(star, actual);
            var pieces = actual.Split(star, StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(5, pieces.Length);
            var indx = pieces.Length - 2;
            var testUri = pieces[indx].Trim();
            Assert.True(Uri.TryCreate(testUri, UriKind.Absolute, out var _));
        }

        [Fact]
        public void ServiceCanGetNavigationUrl()
        {
            var testUri = WilliamsonScriptHelper.GetNavigationUri;
            Assert.True(Uri.TryCreate(testUri, UriKind.Absolute, out var _));
        }

        private static readonly Dictionary<string, string> collection = WilliamsonScriptHelper.ScriptCollection;
    }
}