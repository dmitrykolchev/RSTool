using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSServices
{
    class CommandLineParser
    {
        public Command Command { get; private set; }

        public void Parse(string[] args)
        {
            switch(args[0].ToLower())
            {
                case "show":
                    Command = new CommandShow();
                    break;
                case "createdatasets":
                    Command = new CommandCreateDataSets();
                    break;
                default:
                    throw new InvalidOperationException();
            }
            Command.Parse(args);
        }
    }
}
