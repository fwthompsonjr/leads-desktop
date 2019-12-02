using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegalLead.Changed.Models;

namespace LegalLead.Changed.Classes
{
    public abstract class BuildCommandBase : IBuildCommand
    {
        private string _sourceFileName;

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
            if (!string.IsNullOrEmpty(_sourceFileName)) return;
            if (!File.Exists(sourceFileName)) return;
            ResetFileSource(sourceFileName);
        }

        protected void ResetFileSource(string sourceFileName)
        {
            try
            {
                var sourceData = File.ReadAllText(sourceFileName);
                var changeLog = Newtonsoft.Json.JsonConvert.DeserializeObject<ChangeLog>(sourceData);
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

            var content = Newtonsoft.Json.JsonConvert.SerializeObject(Log, new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat
            });
            using (var sw = new StreamWriter(SourceFile, false))
            {
                sw.Write(content);
                sw.Close();
            }
        }
    }
}
