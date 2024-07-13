using IWshRuntimeLibrary;
using System;
using System.IO;
using System.Reflection;

namespace LegalLead.PublicData.Search.Classes
{
    internal static class ShortcutBuilder
    {
        public static void Build()
        {
            var exePath = Assembly.GetExecutingAssembly().Location;
            var exeFolder = Path.GetDirectoryName(exePath);
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var linkFile = Path.Combine(desktopPath, "legal-lead-search.lnk");
            if (System.IO.File.Exists(linkFile)) { return; }
            var shell = new WshShell();
            try
            {
                var link = shell.CreateShortcut(linkFile);
                var iconLocation = exePath + ",0";
                link.Arguments = "1 2 3";
                link.Description = "Legal Lead Search - v 2.7.1";
                link.iconLocation = iconLocation;
                link.TargetPath = exePath;
                link.WindowStyle = 3;
                link.workingDirectory = exeFolder;
                link.Save();
            }
            catch (Exception)
            {
                // intentional no action on error
            }
        }
    }
}
