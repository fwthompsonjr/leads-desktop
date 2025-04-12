using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
        internal static FormMain mainForm;
        internal static string logDateFormat = "yyyy-MM-ddTHH:mm:ss.fff";
        static internal FormLogin loginForm;
        private static List<WebNavigationKey> _dentonKeys;
        public static List<WebNavigationKey> DentonCustomKeys
        {
            get { return _dentonKeys ??= []; }
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
            using var consoleWriter = new ConsoleWriter();
            var command = Command.CommandStartUp.Commands
                .FirstOrDefault(x =>
                x.Name.Equals(commandName,
                StringComparison.CurrentCultureIgnoreCase)) ?? new Command.MainCommand();

            // get the chrome path in a separate thread
            ThreadStart ts = new(() =>
            {
                var settings = WebUtilities.GetChromeBinary();
                Console.WriteLine("Using {0} as Chrome file location.", settings);
            });
            ts.Invoke();

            consoleWriter.WriteLineEvent += ConsoleWriter_WriteLineEvent;

            Console.SetOut(consoleWriter);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var frm = new FormDentonSetting())
            {
                frm.Save();
            }
            loginForm = new FormLogin
            {
                DialogResult = DialogResult.None
            };
#if DEBUG
            loginForm.DebugMode = true;
#endif
            mainForm = new FormMain();
            command.Execute(mainForm);
        }

        static void ConsoleWriter_WriteLineEvent(object sender, ConsoleWriterEventArgs e)
        {
            var current = new StringBuilder(mainForm.txConsole.Text);
            current.AppendLine(e.Value);
            AppendText(current);
            // Write e.Value to the log file
            WriteToLogFile(e.Value);
        }

        private static void AppendText(StringBuilder sb)
        {
            try
            {
                SetText(sb, true);
            }
            catch (Exception)
            {
                SetTextAlternate(sb);
            }
        }

        private static void SetTextAlternate(StringBuilder sb)
        {
            try
            {
                mainForm.txConsole.Invoke((MethodInvoker)delegate
                {
                    SetText(sb);
                });
            }
            catch
            {
                // suppress error
            }
        }

        private static void SetText(StringBuilder sb, bool throwError = false)
        {
            try
            {
                var textBox = mainForm.txConsole;
                ControlExtensions.Suspend(textBox);
                textBox.Text = sb.ToString();
                textBox.Refresh();
                if (textBox.Text.Length == 0) return;
                textBox.SelectionStart = textBox.Text.Length;
                textBox.ScrollToCaret();
                ControlExtensions.Resume();
            }
            catch (Exception ex)
            {
                if (throwError) throw;
                else Debug.WriteLine(ex.Message);
            }
        }

        private static void WriteToLogFile(string logEntry)
        {
            try
            {
                // Check if the log file size exceeds the maximum size
                FileInfo logFileInfo = new(LogFilePath);
                if (logFileInfo.Exists && logFileInfo.Length > MaxLogFileSize)
                {
                    ArchiveLogFile();
                }
                using StreamWriter writer = new(LogFilePath, true);
                string formattedDate = DateTime.Now.ToString(logDateFormat, CultureInfo.CurrentCulture);
                string formattedLogEntry = $"{formattedDate}: {logEntry}";
                writer.WriteLine(formattedLogEntry);
            }
            catch (Exception)
            {
                // no action of log failure
            }
        }
        static void ArchiveLogFile()
        {
            string archiveFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"log_{DateTime.Now.Ticks}.txt");
            File.Move(LogFilePath, archiveFilePath);
            File.Create(LogFilePath).Dispose(); // Clear the log file
        }
        private static readonly long MaxLogFileSize = 1024 * 1024; // 1 MB for example
        // Static property for the log file path
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
        private static class ControlExtensions
        {
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "<Pending>")]
            public static extern bool LockWindowUpdate(IntPtr hWndLock);

            public static void Suspend(Control control)
            {
                LockWindowUpdate(control.Handle);
            }

            public static void Resume()
            {
                LockWindowUpdate(IntPtr.Zero);
            }

        }
    }
}
