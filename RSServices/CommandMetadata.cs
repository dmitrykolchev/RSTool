using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSServices
{
    class CommandMetadata
    {
        public CommandMetadata(Type commandType)
        {
            CommandType = commandType;
        }
        private string _name;
        public Type CommandType { get; private set; }
        public string Description
        {
            get
            {
                DescriptionAttribute attribute = CommandType.GetCustomAttributes(true).OfType<DescriptionAttribute>().FirstOrDefault();
                return attribute?.Description;
            }
        }
        public string Name
        {
            get
            {
                if (_name == null)
                {
                    CommandAttribute attribute = CommandType.GetCustomAttributes(true).OfType<CommandAttribute>().Single();
                    _name = attribute.Name;
                }
                return _name;
            }
        }

        public Command CreateCommand(string[] args)
        {
            Command command = (Command)Activator.CreateInstance(CommandType);
            command.Metadata = this;
            command.Parse(args);
            return command;
        }
    }
}
