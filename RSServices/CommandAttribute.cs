using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSServices
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute: Attribute
    {
        public string Name { get; set; }
    }
}
