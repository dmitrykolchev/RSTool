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
    //
    /// <summary>
    /// Create-DataSets -url "http://rosatomsql/ReportServer_SAM/ReportService2010.asmx" -source . -destination "/SAM/Данные" -overwrite -datasource "/SAM/SAMDS"
    /// Show
    /// Help
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Microsoft SQL Server Reporting Services deployment tool");
                Console.WriteLine("(c) 2016 Dmitry Kolchev");
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                CommandLineParser commandLine = new CommandLineParser();
                commandLine.Parse(args);
                CommandExecutionContext context = new CommandExecutionContext { Parser = commandLine };
                commandLine.Command.Execute(context);
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
