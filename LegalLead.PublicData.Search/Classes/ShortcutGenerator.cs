﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    var update = UpdateVersionNumber(setupFile, version);
                    var exists = DoesAppShortcutExist();
                    if (exists || !update) return;
                    DeleteAppShortcut();
                    using (var process = new Process())
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

        private static bool DoesAppShortcutExist()
        {
            try
            {
                var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var linkFile = Path.Combine(desktopFolder, "legal-lead-search.lnk");
                return File.Exists(linkFile);
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static void DeleteAppShortcut()
        {
            try
            {
                var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var linkFile = Path.Combine(desktopFolder, "legal-lead-search.lnk");
                if (File.Exists(linkFile)) File.Delete(linkFile);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        private static bool UpdateVersionNumber(string targetFile, string config)
        {
            const char dot = '.';
            const string line1 = "VersionName = \"{0}\"";
            const string line2 = "VersionNumber = VersionName & \"{0}\"";
            var ds = dot.ToString();
            if (!File.Exists(targetFile)) return false;
            if (string.IsNullOrEmpty(config)) return false;
            if (!config.Contains(ds)) return false;
            var list = config.Split(dot).ToList();
            if (list.Count != 4) return false;
            var content = new StringBuilder(File.ReadAllText(targetFile));
            var versionId = string.Join(ds, list.Take(2));
            var find1 = GetVersionReplacement(line1, "2.8");
            var find2 = GetVersionReplacement(line2, ".0");
            var revision = list[2];
            var replace1 = GetVersionReplacement(line1, versionId);
            var replace2 = GetVersionReplacement(line2, $".{revision}");
            var hasReplacements = content.ToString().Contains(find1) ||
                content.ToString().Contains(find2);
            if (!hasReplacements) return false;
            if (string.IsNullOrEmpty(ExeFileName(versionId, config))) return false;
            if (find1 == replace1 && find2 == replace2) return false;
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
            if (!isUpdated) return false;
            File.WriteAllText(targetFile, content.ToString());
            return true;
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
        private static string ExeFileName(string versionNumber, string fileVersion)
        {
            var localDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrEmpty(localDataFolder) || !Directory.Exists(localDataFolder)) return string.Empty;
            var appDir = Path.Combine(localDataFolder, "LegalLead");
            if (string.IsNullOrEmpty(appDir) || !Directory.Exists(appDir)) return string.Empty;
            var versionDir = Path.Combine(appDir, versionNumber);
            if (string.IsNullOrEmpty(versionDir) || !Directory.Exists(versionDir)) return string.Empty;
            var applicationFile = Path.Combine(versionDir, "LegalLead.PublicData.Search.exe");
            if (string.IsNullOrEmpty(applicationFile) || !File.Exists(applicationFile)) return string.Empty;
            var versionInfo = FileVersionInfo.GetVersionInfo(applicationFile).FileVersion;
            if (!versionInfo.Equals(fileVersion, StringComparison.OrdinalIgnoreCase)) return string.Empty;
            return applicationFile;
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
        private static readonly object locker = new();
    }
}
