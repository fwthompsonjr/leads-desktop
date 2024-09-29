using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Classes
{
    public static class DallasScriptHelper
    {

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
        private static string scriptFileName;
        private static string scriptContent;
        private static string ScriptFileName()
        {
            var appFolder =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var xmlFolder = Path.Combine(appFolder, "xml");
            var scriptFile = Path.Combine(xmlFolder, "dallasScriptingHelp.txt");
            return scriptFile;
        }

        private static string ScriptContent()
        {
            var content = File.ReadAllText(GetScriptFileName);
            content = content.Replace('\\', '~').Replace("~~", "~");
            return content;
        }
    }
}
