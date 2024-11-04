using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Tools;

namespace Thompson.RecordSearch.Utility.Classes
{
    public static class FortBendScriptHelper
    {
        public static Dictionary<string, string> ScriptCollection
        {
            get
            {
                if (collection != null) return collection;
                collection = ScriptHelpBuilder.GetCollection(script_content);
                return collection;
            }
        }
        public static string GetNavigationUri
        {
            get
            {
                if (!string.IsNullOrEmpty(navigation_content)) return navigation_content;
                navigation_content = FindNavigation();
                return navigation_content;
            }
        }

        private static string FindNavigation()
        {
            const char star = '*';
            const string scriptName = "get navigation url";
            var isFound = ScriptCollection.TryGetValue(scriptName, out var actual);
            if (!isFound) return string.Empty;
            var pieces = actual.Split(star, StringSplitOptions.RemoveEmptyEntries);
            var indx = pieces.Length - 2;
            return pieces[indx].Trim();
        }
        private static Dictionary<string, string> collection = null;
        private static readonly string script_content = Properties.Resources.fortbend_scripts;
        private static string navigation_content = null;
    }
}