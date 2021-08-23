using LegalLead.Changed.Models;
using System;
using System.IO;
using System.Linq;

namespace LegalLead.Changed.Commands
{
    public abstract class BaseUserCommand : IUserCommand
    {

        private string _sourceFileName;
        public virtual int Index => 0;
        public string Name
        {
            get { return GetType().Name; }
        }


        /// <summary>
        /// Gets the Source file for this command
        /// </summary>
        public virtual string SourceFile => _sourceFileName ?? string.Empty;

        /// <summary>
        /// Gets the log file associated to application changes
        /// </summary>
        public ChangeLog Log { get; private set; }

        /// <summary>
        /// Gets the latest version associated with the last change
        /// </summary>
        public Models.Version LatestVersion { get; private set; }

        public void SetSource(string sourceFileName)
        {
            if (!string.IsNullOrEmpty(_sourceFileName)) return;
            if (!File.Exists(sourceFileName)) return;
            ResetFileSource(sourceFileName);
        }

        public abstract void Execute();

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

    }
}
