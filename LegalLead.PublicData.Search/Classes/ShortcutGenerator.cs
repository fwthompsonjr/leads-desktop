using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LegalLead.PublicData.Search.Classes
{
    public static class ShortcutGenerator
    {
        public static void Generate()
        {
            lock (locker)
            {
                try
                {
                    if (hasRun) return;
                    var setupFile = SetupFile;
                    if (string.IsNullOrEmpty(setupFile)) return;
                    var version = CurrentAssembly.GetName().Version.ToString();
                    UpdateVersionNumber(setupFile, version);
                    using (var process = new System.Diagnostics.Process())
                    {
                        process.StartInfo.FileName = "cscript";
                        process.StartInfo.Arguments = $"\"{setupFile}\"";
                        process.StartInfo.Verb = "runas";
                        process.Start();
                        process.WaitForExit();
                    }
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    hasRun = true;
                } 
            }
        }

        private static void UpdateVersionNumber(string targetFile, string config)
        {
            const char dot = '.';
            const string line1 = "VersionName = \"{0}\"";
            const string line2 = "VersionNumber = VersionName & \"{0}\"";
            var ds = dot.ToString();
            if (!File.Exists(targetFile)) return;
            if (string.IsNullOrEmpty(config)) return;
            if (!config.Contains(ds)) return;
            var list = config.Split(dot).ToList();
            var content = new StringBuilder(File.ReadAllText(targetFile));
            var find1 = GetVersionReplacement(line1, "2.8");
            var find2 = GetVersionReplacement(line2, ".0");
            var replace1 = GetVersionReplacement(line1, string.Join(ds, list.Take(2)));
            var replace2 = GetVersionReplacement(line2, $".{list[3]}");
            var hasReplacements = content.ToString().Contains(find1) ||
                content.ToString().Contains(find2);
            if (!hasReplacements) return;
            var replacements = new Dictionary<string, string>()
            {
                { find1, replace1 },
                { find2, replace2 },
            };
            var isUpdated = false;
            foreach (var replacement in replacements)
            {
                if (replacement.Key == replacement.Value) continue;
                content.Replace(replacement.Key, replacement.Value);
                isUpdated = true;
            }
            if (!isUpdated) return;
            File.WriteAllText(targetFile, content.ToString());
        }
        private static string GetVersionReplacement(string pattern, string replacement)
        {
            return string.Format(pattern, replacement);
        }
        private static string SetupFile
        {
            get
            {
                if (setupFileName != null) return setupFileName;
                setupFileName = SetupFileName();
                return setupFileName;
            }
        }

        private static string SetupFileName()
        {
            var setupFolder = SetupDirectoyName();
            if (string.IsNullOrEmpty(setupFolder) ||
                !Directory.Exists(setupFolder)) return string.Empty;
            var setupFile = Path.Combine(setupFolder, "generate-shortcut.vbs");
            if (string.IsNullOrEmpty(setupFile) ||
                !File.Exists(setupFile)) return string.Empty;
            return setupFile;
        }

        private static string SetupDirectoyName()
        {
            var appFolder = Path.GetDirectoryName(CurrentAssembly.Location);
            var setupFolder = Path.Combine(appFolder, "_setup");
            if (!Directory.Exists(setupFolder)) return string.Empty;
            return setupFolder;
        }
        private static Assembly CurrentAssembly
        {
            get
            {
                if (executingAssembly != null) return executingAssembly;
                executingAssembly = Assembly.GetExecutingAssembly();
                return executingAssembly;
            }
        }
        private static string setupFileName = null;
        private static Assembly executingAssembly = null;
        private static bool hasRun = false;
        private static readonly object locker = new object();
    }
}
