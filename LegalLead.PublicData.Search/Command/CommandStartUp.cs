﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LegalLead.PublicData.Search.Command
{
    public static class CommandStartUp
    {

        private static IEnumerable<CommandBase> textCommands;

        public static IEnumerable<CommandBase> Commands { get { return textCommands ?? (textCommands = GetCommands()); } }

        private static IEnumerable<CommandBase> GetCommands()
        {
            var interfaceType = typeof(CommandBase);
            var commands = AppDomain.CurrentDomain.GetAssemblies()
              .SelectMany(x => x.GetTypes())
              .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
              .Select(x => Activator.CreateInstance(x) as CommandBase).ToList();
            commands.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
            return commands;
        }
    }
}
