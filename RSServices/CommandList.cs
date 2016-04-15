using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSServices.RS2010;
using System.ComponentModel;

namespace RSServices
{
    [Command(Name = "List")]
    [Description("Позволяет просматривать текущий список объектов")]
    public class CommandList : CommandBase
    {
        public CommandList() { }

        protected override void ExecuteOverride(CommandExecutionContext context)
        {
            EnumerateItems("/", 0);
        }
        [CommandArgument(Optional = true, HasValue = false)]
        public string Full { get; set; }
        public bool IsFullSpecified { get; set; }
        [CommandArgument(Optional = true, HasValue = false)]
        public string References { get; set; }
        public bool IsReferencesSpecified { get; set; }
        private void EnumerateItems(string path, int level)
        {
            CatalogItem[] items = Service.ListChildren(path, false);
            foreach (CatalogItem item in items)
            {
                var indent = new string(' ', level * 4);
                WriteItemInfo(item, indent);

                if (item.TypeName == "Folder")
                    EnumerateItems(item.Path, level + 1);

                if (IsReferencesSpecified)
                {
                    if (item.TypeName == "DataSet")
                    {
                        Console.WriteLine($"{indent}    References");
                        ItemReferenceData[] references = Service.GetItemReferences(item.Path, "DataSource");
                        foreach (ItemReferenceData reference in references)
                            Console.WriteLine($"{indent}    {reference.Name} {reference.Reference} <{reference.ReferenceType}>");
                    }
                    else if (item.TypeName == "Report")
                    {
                        Console.WriteLine($"{indent}    References");
                        ItemReferenceData[] references = Service.GetItemReferences(item.Path, "DataSet");
                        foreach (ItemReferenceData reference in references)
                            Console.WriteLine($"{indent}    {reference.Name} {reference.Reference} <{reference.ReferenceType}>");
                        references = Service.GetItemReferences(item.Path, "DataSource");
                        foreach (ItemReferenceData reference in references)
                            Console.WriteLine($"{indent}    {reference.Name} {reference.Reference} <{reference.ReferenceType}>");
                    }
                }
            }
        }
        private void WriteItemInfo(CatalogItem item, string indent)
        {
            if (IsFullSpecified)
            {
                if (item.TypeName == "Folder")
                    Console.WriteLine($"{indent}{item.ID} /{item.Name} {item.CreationDate} {item.CreatedBy} {item.ModifiedDate} {item.ModifiedBy}");
                else
                    Console.WriteLine($"{indent}{item.ID} {item.Name} <{item.TypeName}> {item.CreationDate} {item.CreatedBy} {item.ModifiedDate} {item.ModifiedBy}");
            }
            else
            {
                if (item.TypeName == "Folder")
                    Console.WriteLine($"{indent}/{item.Name}");
                else
                    Console.WriteLine($"{indent}{item.Name} <{item.TypeName}>");
            }
        }
    }
}
