using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace RSServices
{
    class CommandArgumentMetadata
    {
        public int Ordinal { get; set; }
        public string Name { get; set; }
        public PropertyInfo Property { get; set; }
        public PropertyInfo IsPropertySpecified { get; set; }
        public string Description
        {
            get
            {
                DescriptionAttribute attribute = Property.GetCustomAttribute<DescriptionAttribute>();
                return attribute?.Description;
            }
        }
        public CommandArgumentAttribute Attributes { get; set; }
    }
}
