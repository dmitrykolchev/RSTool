using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RSServices.RS2010;
using System.ComponentModel;

namespace RSServices
{
    [Command(Name = "Create-DataSets")]
    [Description("Позволяет создать новые или обновить существующие разделяемые наборы данных")]
    public class CommandCreateDataSets : CommandBase
    {
        public CommandCreateDataSets()
        {
        }
        public bool IsSourceSpecified { get; set; }
        [CommandArgument(HasValue = true)]
        public string Source { get; set; }
        public bool IsDestinationSpecified { get; set; }
        [CommandArgument(HasValue = true)]
        public string Destination { get; set; }
        public bool IsOverwriteSpecified { get; set; }
        [CommandArgument(HasValue = false, Optional = true)]
        public string Overwrite { get; set; }
        public bool IsDataSourceSpecified { get; set; }
        [CommandArgument(HasValue = true, Optional = true)]
        public string DataSource { get; set; }
        protected override void ExecuteOverride(CommandExecutionContext context)
        {
            string[] files = Directory.GetFiles(Source, "*.rsd", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                byte[] buffer;
                using (MemoryStream output = new MemoryStream())
                using (FileStream input = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    input.CopyTo(output);
                    buffer = output.ToArray();
                }
                string name = Path.GetFileNameWithoutExtension(file);
                CreateDataSet(name, buffer);
            }
        }
        private void CreateDataSet(string name, byte[] buffer)
        {
            Console.WriteLine($"Creating dataset '{name}'");
            Warning[] warnings;
            CatalogItem result = Service.CreateCatalogItem(
                "DataSet",
                name,
                Destination,
                IsOverwriteSpecified,
                buffer,
                new Property[] {
                    new Property { Name= "Description", Value=$"Источник данных - {name}" }
                },
                out warnings
            );
            if (warnings != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (Warning warning in warnings)
                {
                    Console.WriteLine($"WARNING ({warning.Code}): {warning.Message}");
                }
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.WriteLine($"DataSet '{result.Name}' successfully created in '{result.Path}'");
            if (IsDataSourceSpecified)
            {
                Console.WriteLine("Updating DataSource reference");
                Service.SetItemReferences(result.Path, new ItemReference[] {
                    new ItemReference { Name = "DataSetDataSource", Reference = DataSource
                    }
                });
            }
            Console.WriteLine();
        }
    }
}
