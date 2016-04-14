using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSServices
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandArgumentAttribute : Attribute
    {
        public string Name { get; set; }
        public bool Optional { get; set; }
        public bool HasValue { get; set; }
    }
}
