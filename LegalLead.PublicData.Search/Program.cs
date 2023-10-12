using LegalLead.PublicData.Search.Command;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility;
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
            var commandName = ConfigurationManager.AppSettings[CommonKeyIndexes.FormStartup] ??
                CommonKeyIndexes.FormNameMain.ToLower(CultureInfo.CurrentCulture);
            using (var consoleWriter = new ConsoleWriter())
            {
                var command = Command.CommandStartUp.Commands
                    .FirstOrDefault(x =>
                    x.Name.Equals(commandName,
                    StringComparison.CurrentCultureIgnoreCase));
                if (command == null)
                {
                    command = new Command.MainCommand();
                }

                // get the chrome path in a separate thread
                ThreadStart ts = new ThreadStart(() =>
                {
                    var settings = WebUtilities.GetChromeBinary();
                    Console.WriteLine("Using {0} as Chrome file location.", settings);
                });
                ts.Invoke();

                consoleWriter.WriteEvent += ConsoleWriter_WriteEvent;
                consoleWriter.WriteLineEvent += ConsoleWriter_WriteLineEvent;

                Console.SetOut(consoleWriter);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (var frm = new FormDentonSetting())
                {
                    frm.Save();
                }
                mainForm = new FormMain();
                HarrisCriminalUpdate.Update();
                command.Execute(mainForm);
            }
        }

        static void ConsoleWriter_WriteLineEvent(object sender, ConsoleWriterEventArgs e)
        {
            var current = new StringBuilder(mainForm.txConsole.Text);
            current.AppendLine(e.Value);
            AppendText(current);
        }

        static void ConsoleWriter_WriteEvent(object sender, ConsoleWriterEventArgs e)
        {
            var current = new StringBuilder(mainForm.txConsole.Text);
            current.Append(e.Value);
            AppendText(current);
        }

        private static void AppendText(StringBuilder sb)
        {
            try
            {
                mainForm.txConsole.Text = sb.ToString();
                mainForm.txConsole.Refresh();
            }
            catch (Exception)
            {

                mainForm.txConsole.Invoke((MethodInvoker)delegate
                {
                    mainForm.txConsole.Text = sb.ToString();
                    mainForm.txConsole.Refresh();
                });
            }
        }
    }
}
