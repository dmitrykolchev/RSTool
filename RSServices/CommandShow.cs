using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSServices.RS2010;

namespace RSServices
{
    [Command(Name = "Show")]
    public class CommandShow : Command
    {
        public CommandShow() { }

        protected override void ExecuteOverride()
        {
            EnumerateItems(Service, "/", 0);
        }
        private static void EnumerateItems(ReportingService2010 rs, string path, int level)
        {
            CatalogItem[] items = rs.ListChildren(path, false);
            foreach (CatalogItem item in items)
            {
                var indent = new string(' ', level * 4);
                if (item.TypeName == "Folder")
                    Console.WriteLine($"{indent}/{item.Name}");
                else
                    Console.WriteLine($"{indent}{item.Name} <{item.TypeName}>");
                if (item.TypeName == "Folder")
                    EnumerateItems(rs, item.Path, level + 1);
                else if (item.TypeName == "DataSet")
                {
                    Console.WriteLine($"{indent}    References");
                    ItemReferenceData[] references = rs.GetItemReferences(item.Path, "DataSource");
                    foreach (ItemReferenceData reference in references)
                        Console.WriteLine($"{indent}    {reference.Name} {reference.Reference} <{reference.ReferenceType}>");
                }
                else if (item.TypeName == "Report")
                {
                    Console.WriteLine($"{indent}    References");
                    ItemReferenceData[] references = rs.GetItemReferences(item.Path, "DataSet");
                    foreach (ItemReferenceData reference in references)
                        Console.WriteLine($"{indent}    {reference.Name} {reference.Reference} <{reference.ReferenceType}>");
                    references = rs.GetItemReferences(item.Path, "DataSource");
                    foreach (ItemReferenceData reference in references)
                        Console.WriteLine($"{indent}    {reference.Name} {reference.Reference} <{reference.ReferenceType}>");

                }
            }
        }
    }
}
