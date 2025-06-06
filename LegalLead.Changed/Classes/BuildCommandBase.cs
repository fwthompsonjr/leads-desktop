﻿using LegalLead.Changed.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LegalLead.Changed.Classes
{
    public abstract class BuildCommandBase : IBuildCommand
    {
        private string _sourceFileName;

        public string Name
        {
            get { return GetType().Name; }
        }
        /// <summary>
        /// Determines the order of execution for command
        /// </summary>
        public abstract int Index { get; }

        /// <summary>
        /// Gets the Source file for this command
        /// </summary>
        public virtual string SourceFile => _sourceFileName ?? string.Empty;

        /// <summary>
        /// Gets the log file associated to application changes
        /// </summary>
        public virtual ChangeLog Log { get; private set; }

        public Models.Version LatestVersion { get; private set; }

        /// <summary>
        /// Executes associated command action
        /// </summary>
        public abstract bool Execute();

        public void SetSource(string sourceFileName)
        {
            if (!string.IsNullOrEmpty(_sourceFileName))
            {
                return;
            }

            if (!File.Exists(sourceFileName))
            {
                return;
            }

            ResetFileSource(sourceFileName);
        }

        protected void ResetFileSource(string sourceFileName)
        {
            try
            {
                var changeLog = JsReader.Read<ChangeLog>(sourceFileName);
                var lastChange = changeLog.Versions.LastOrDefault(v => v.Fixes.Any(f => f.CanPublish));
                _sourceFileName = sourceFileName;
                Log = changeLog;
                LatestVersion = lastChange;
            }
            catch (Newtonsoft.Json.JsonSerializationException)
            {
                // do nothing
            }
            catch (Exception)
            {

                throw;
            }
        }


        protected void ReSerialize()
        {
            JsReader.Write(Log, SourceFile);
        }

        protected bool CanExecute()
        {

            if (string.IsNullOrEmpty(SourceFile))
            {
                throw new InvalidOperationException();
            }

            if (Log == null)
            {
                throw new InvalidOperationException();
            }

            return true;
        }

        protected static string ReadMeFileName()
        {
            var appFolder =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var readMeFile = Path.Combine(appFolder, "ReadMe.txt");
            return readMeFile;
        }

        protected static string SolutionReadMe()
        {
            const string solutionName = "LegalLead";
            const string utilityName = "Thompson.RecordSearch.Utility";
            var appFolder =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dir = new DirectoryInfo(appFolder);
            while (!dir.Name.StartsWith(solutionName, StringComparison.CurrentCultureIgnoreCase))
            {
                dir = new DirectoryInfo(dir.Parent.FullName);
            }
            if (dir.Parent == null)
                throw new InvalidOperationException();
            dir = dir.Parent;
            return Path.Combine(dir.Parent.FullName, utilityName, @"xml\ReadMe.txt");
        }
    }
}
