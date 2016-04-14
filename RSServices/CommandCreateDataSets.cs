using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RSServices.RS2010;

namespace RSServices
{
    public class CommandCreateDataSets: Command
    {
        public CommandCreateDataSets()
        {
        }
        public bool IsSourceSpecified { get; set; }
        public string Source { get; set; }
        public bool IsDestinationSpecified { get; set; }
        public string Destination { get; set; }
        public bool IsOverwriteSpecified { get; set; }
        public bool IsDataSourceSpecified { get; set; }
        public string DataSource { get; set; }
        protected override ArgumentOptions ValidateCommandArgument(string name)
        {
            switch(name)
            {
                case "-source":
                    IsSourceSpecified = true;
                    return ArgumentOptions.HasValue;
                case "-destination":
                    IsDestinationSpecified = true;
                    return ArgumentOptions.HasValue;
                case "-datasource":
                    IsDataSourceSpecified = true;
                    return ArgumentOptions.HasValue | ArgumentOptions.Optional;
                case "-overwrite":
                    IsOverwriteSpecified = true;
                    return ArgumentOptions.Optional;
            }
            return base.ValidateCommandArgument(name);
        }
        protected override void ValidateCommand()
        {
            if (!IsSourceSpecified || !IsDestinationSpecified)
                throw new InvalidOperationException();
            base.ValidateCommand();
        }
        protected override void SetArgumentValue(string name, string value)
        {
            switch (name)
            {
                case "-source":
                    this.Source = value;
                    break;
                case "-destination":
                    this.Destination = value;
                    break;
                case "-datasource":
                    this.DataSource = value;
                    break;
                default:
                    base.SetArgumentValue(name, value);
                    break;
            }
        }
        protected override void ExecuteOverride()
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
