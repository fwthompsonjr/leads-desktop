using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    static class Program
    {
        static FormMain mainForm;
        private static List<WebNavigationKey> _dentonKeys;
        public static List<WebNavigationKey> DentonCustomKeys
        {
            get { return _dentonKeys ?? (_dentonKeys = new List<WebNavigationKey>()); }
            set { _dentonKeys = value; }
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var commandName = ConfigurationManager.AppSettings["form-startup"] ?? "main";
            using (var consoleWriter = new ConsoleWriter())
            {
                var command = Command.CommandStartUp.Commands
                    .ToList().FirstOrDefault(x =>
                    x.Name.Equals(commandName,
                    StringComparison.CurrentCultureIgnoreCase));
                if (command == null)
                {
                    command = new Command.MainCommand();
                }
                consoleWriter.WriteEvent += consoleWriter_WriteEvent;
                consoleWriter.WriteLineEvent += consoleWriter_WriteLineEvent;

                Console.SetOut(consoleWriter);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                new FormDentonSetting().Save();
                mainForm = new FormMain();
                command.Execute(mainForm);
            }
        }

        static void consoleWriter_WriteLineEvent(object sender, ConsoleWriterEventArgs e)
        {
            var current = new StringBuilder(mainForm.txConsole.Text);
            current.AppendLine(e.Value);
            mainForm.txConsole.Text = current.ToString();
            mainForm.txConsole.Refresh();
        }

        static void consoleWriter_WriteEvent(object sender, ConsoleWriterEventArgs e)
        {
            var current = new StringBuilder(mainForm.txConsole.Text);
            current.Append(e.Value);
            mainForm.txConsole.Text = current.ToString();
        }
    }
}
