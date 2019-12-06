using System;

namespace LegalLead.Changed.Classes
{
    public class CommandLoadLog : BuildCommandBase
    {
        public override int Index => 1;

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(SourceFile))
                throw new InvalidOperationException();

            if(Log == null)
            {
                SetSource(SourceFile);
            }

            Console.WriteLine("{0} change log file.", Log == null ? "Failed to load" : "Successfully loaded");
            return Log != null;
        }
    }
}
