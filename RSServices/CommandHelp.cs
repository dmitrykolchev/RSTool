using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSServices
{
    [Command(Name ="Help")]
    [Description("Показать информацию о доступных командах")]
    class CommandHelp: Command
    {
        public CommandHelp()
        {
        }
        protected override void ExecuteOverride(CommandExecutionContext context)
        {
            foreach(CommandMetadata metadata in context.Parser.Metadata.Values)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Command command = metadata.CreateCommand(new string[] { "", "-help" });
                command.Execute(context);
                Console.WriteLine();
            }
        }
        protected override void ValidateCommand()
        {
        }
    }
}
