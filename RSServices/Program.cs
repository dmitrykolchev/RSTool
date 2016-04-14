using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSServices.RS2010;
using System.Net;
using System.IO;

namespace RSServices
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                CommandLineParser commandLine = new CommandLineParser();
                commandLine.Parse(args);
                commandLine.Command.Execute();
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine("------------------------------------------------------------------------");
                Console.WriteLine(ex.ToString());
            }
        }


    }
}
