using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;

namespace Thompson.RecordSearch.Utility.Classes
{
    public static class DallasScriptHelper
    {
        public static List<NavigationStep> NavigationSteps
        {
            get
            {
                if (steps != null) return steps;
                var itm = JsonConvert.DeserializeObject<DallasJs>(GetJsonContent);
                steps = new List<NavigationStep>();
                if (itm != null) steps.AddRange(itm.Steps);
                return steps;
            }
        }
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
        public static string GetScriptFileName
        {
            get
            {
                if (!string.IsNullOrEmpty(scriptFileName)) return scriptFileName;
                scriptFileName = ScriptFileName();
                return scriptFileName;
            }
        }

        public static string GetJsonContent
        {
            get
            {
                if (!string.IsNullOrEmpty(jsonContent)) return jsonContent;
                jsonContent = File.ReadAllText(GetJsonFileName);
                return jsonContent;
            }
        }
        public static string GetJsonFileName
        {
            get
            {
                if (!string.IsNullOrEmpty(jsonFileName)) return jsonFileName;
                jsonFileName = JsonFileName();
                return jsonFileName;
            }
        }

        private static Dictionary<string, string> collection = null;
        private static List<string> scripts = null;
        private static List<NavigationStep> steps = null;
        private static string scriptFileName;
        private static string scriptContent;
        private static string jsonFileName;
        private static string jsonContent;

        private static string ScriptFileName()
        {
            var appFolder =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var xmlFolder = Path.Combine(appFolder, "xml");
            var scriptFile = Path.Combine(xmlFolder, "dallasScriptingHelp.txt");
            return scriptFile;
        }


        private static string JsonFileName()
        {
            var appFolder =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var xmlFolder = Path.Combine(appFolder, "xml");
            var scriptFile = Path.Combine(xmlFolder, "dallasCountyMapping.json");
            return scriptFile;
        }
        private static string ScriptContent()
        {
            var content = File.ReadAllText(GetScriptFileName);
            return content;
        }
        private static List<string> ScriptBlocks()
        {
            var content = GetScriptContent;
            var arr = content.Split('~').ToList();
            return arr;
        }


        private sealed class DallasJs
        {
            [JsonProperty("steps")]
            public List<NavigationStep> Steps { get; set; }
        }
    }
}
