using System.Collections.Generic;
using System.Linq;
using Thompson.RecordSearch.Utility.Extensions;

namespace Thompson.RecordSearch.Utility.Classes
{
    public static class HidalgoScriptHelper
    {
        public static Dictionary<string, string> ScriptCollection
        {
            get
            {
                if (collection != null) return collection;
                collection = new Dictionary<string, string>();
                collection.AppendScripts(Scripts);
                return collection;
            }
        }
        public static List<string> Scripts
        {
            get
            {
                if (scripts != null) return scripts;
                scripts = ScriptBlocks();
                return scripts;
            }
        }
        public static string GetScriptContent
        {
            get
            {
                if (!string.IsNullOrEmpty(scriptContent)) return scriptContent;
                scriptContent = ScriptContent();
                return scriptContent;
            }
        }

        private static Dictionary<string, string> collection = null;
        private static List<string> scripts = null;
        private static string scriptContent;


        private static string ScriptContent()
        {
            var content = Properties.Resources.hidalgo_scripts;
            return content;
        }
        private static List<string> ScriptBlocks()
        {
            var content = GetScriptContent;
            var arr = content.Split('~').ToList();
            return arr;
        }
    }
}