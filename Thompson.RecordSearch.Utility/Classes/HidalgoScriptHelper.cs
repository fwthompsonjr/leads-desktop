using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Tools;

namespace Thompson.RecordSearch.Utility.Classes
{
    public static class HidalgoScriptHelper
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

        private static Dictionary<string, string> collection = null;
        private static readonly string script_content = Properties.Resources.hidalgo_scripts;
    }
}