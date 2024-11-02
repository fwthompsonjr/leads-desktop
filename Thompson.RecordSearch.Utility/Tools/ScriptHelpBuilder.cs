using System.Collections.Generic;
using System.Linq;
using Thompson.RecordSearch.Utility.Extensions;

namespace Thompson.RecordSearch.Utility.Tools
{
    internal static class ScriptHelpBuilder
    {
        public static Dictionary<string, string> GetCollection(string content)
        {
            if (string.IsNullOrEmpty(content)) return new Dictionary<string, string>();
            var arr = content.Split('~').ToList();
            var collection = new Dictionary<string, string>();
            collection.AppendScripts(arr);
            return collection;
        }
    }
}
