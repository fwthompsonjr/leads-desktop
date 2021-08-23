using LegalLead.Changed.Classes;
using LegalLead.Changed.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LegalLead.Changed
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                Console.WriteLine("Executing post build actions.");
                if(args.Length > 1)
                {
                    Evaluate(args[0], args[1]);
                }
                if (!string.IsNullOrEmpty(args[0]))
                {
                    var srcFile = args[0];
                    Console.WriteLine("Evaluating command line file: {0}", srcFile);
                    if (!File.Exists(srcFile)) return;
                    var commands = GetClasses(srcFile);
                    commands.ForEach(c => {
                        Console.WriteLine("Executing {0}", c.Name);
                        c.Execute(); 
                    });
                }

            }
        }

        private static void Evaluate(string sourceFileName, string argument)
        {
            if (string.IsNullOrEmpty(argument))
                return;

            var type = typeof(IUserCommand);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .ToList();
            var commands = new List<IUserCommand>();
            types.ForEach(f =>
            {
                var command = (IUserCommand)Activator.CreateInstance(f);
                if(command.Name.Equals(argument, StringComparison.OrdinalIgnoreCase))
                {
                    commands.Add(command);
                }
            });
            if (!commands.Any())
            {
                return;
            }
            commands.ForEach(c => c.SetSource(sourceFileName));
            commands.ForEach(c => c.Execute());
        }

        private static List<IBuildCommand> GetClasses(string sourceFileName) 
        {
            var type = typeof(IBuildCommand);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .ToList();
            var commands = new List<IBuildCommand>();
            types.ForEach(f => commands.Add((IBuildCommand)Activator.CreateInstance(f)));
            commands.ForEach(c => c.SetSource(sourceFileName));
            commands = commands.FindAll(c => c.Index > 0);
            commands.Sort((a,b) => a.Index.CompareTo(b.Index));
            return commands;
        }
    }
}
