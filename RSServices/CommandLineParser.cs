using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace RSServices
{
    public class CommandLineParser
    {
        private Dictionary<string, CommandMetadata> _commands;
        public Command Command { get; private set; }

        private void Initialize()
        {
            if (_commands == null)
            {
                var commandTypes = GetType().Assembly.GetTypes().Where(t => typeof(Command).IsAssignableFrom(t));
                _commands = new Dictionary<string, CommandMetadata>(StringComparer.InvariantCultureIgnoreCase);
                foreach (Type commandType in commandTypes)
                {
                    if (!commandType.IsAbstract)
                    {
                        var metadata = new CommandMetadata(commandType);
                        _commands.Add(metadata.Name, metadata);
                    }
                }
            }
        }
        internal Dictionary<string, CommandMetadata> Metadata
        {
            get
            {
                return _commands;
            }
        }

        public void Parse(string[] args)
        {
            Initialize();
            CommandMetadata metadata;
            if(args.Length == 0)
            {
                Command = _commands["help"].CreateCommand(new string[] { "help" });
            }
            else if (_commands.TryGetValue(args[0], out metadata))
            {
                Command = metadata.CreateCommand(args);
            }
            else
            {
                throw new InvalidOperationException($"unknown command '{args[0]}'");
            }
        }
    }
}
