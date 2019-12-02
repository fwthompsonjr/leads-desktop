using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalLead.Changed.Classes
{
    public class CommandLoadLog : BuildCommandBase
    {
        public override int Index => 1;

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(SourceFile))
                throw new ArgumentOutOfRangeException("SourceFile");

            if(Log == null)
            {
                SetSource(SourceFile);
            }

            Console.WriteLine("{0} change log file.", Log == null ? "Failed to load" : "Successfully loaded");
            return Log != null;
        }
    }
}
