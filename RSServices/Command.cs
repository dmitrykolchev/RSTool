using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSServices.RS2010;
using System.Net;
using System.Reflection;

namespace RSServices
{
    [Flags]
    public enum ArgumentOptions
    {
        None = 0,
        HasValue = 1,
        Optional = 2
    }
    public abstract class Command
    {
        private Dictionary<string, CommandArgumentMetadata> _metadata;

        protected Command()
        {
        }
        void InitializeMetadata()
        {
            if (_metadata == null)
            {
                _metadata = new Dictionary<string, CommandArgumentMetadata>(StringComparer.InvariantCultureIgnoreCase);

                var properties = GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(t => t.GetCustomAttribute<CommandArgumentAttribute>() != null);
                foreach (var property in properties)
                {
                    CommandArgumentAttribute attribute = property.GetCustomAttribute<CommandArgumentAttribute>();
                    string isPropertySpecifiedName = "Is" + property.Name + "Specified";
                    PropertyInfo isPropertySpecified = GetType().GetProperty(isPropertySpecifiedName,
                        BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic);
                    CommandArgumentMetadata metadata = new CommandArgumentMetadata
                    {
                        Name = string.IsNullOrEmpty(attribute.Name) ? property.Name.ToLower() : attribute.Name.ToLower(),
                        Property = property,
                        IsPropertySpecified = isPropertySpecified,
                        Attributes = attribute
                    };
                    this._metadata.Add(metadata.Name, metadata);
                }
            }
        }
        [CommandArgument(Name = "Help", HasValue = false, Optional = true)]
        public string Help { get; set; }
        public bool IsHelpSpecified { get; set; }
        [CommandArgument(HasValue = true)]
        public string Url { get; set; }
        public bool IsUrlSpecified { get; set; }
        public void Parse(string[] args)
        {
            InitializeMetadata();
            for (int index = 1; index < args.Length; ++index)
            {
                if (args[index][0] == '-' || args[index][0] == '/')
                {
                    string argumentName = args[index].Substring(1).ToLower();
                    CommandArgumentMetadata metadata;
                    if (!_metadata.TryGetValue(argumentName, out metadata))
                        throw new InvalidOperationException("unknown argument");
                    metadata.IsPropertySpecified.SetValue(this, true);
                    if (metadata.Attributes.HasValue)
                    {
                        metadata.Property.SetValue(this, args[index + 1]);
                        index++;
                    }
                }
            }
            ValidateCommand();
        }
        private void ValidateCommand()
        {
            CommandArgumentMetadata helpMetadata;
            if (!_metadata.TryGetValue("help", out helpMetadata))
            {
                foreach (CommandArgumentMetadata metadata in _metadata.Values)
                {
                    if (!metadata.Attributes.Optional)
                    {
                        if (!(bool)metadata.IsPropertySpecified.GetValue(this))
                            throw new InvalidOperationException($"required argument {metadata.Name} not specified");
                    }
                }
            }
        }
        private ReportingService2010 _service;
        public ReportingService2010 Service => _service;
        public void Execute()
        {
            if (IsHelpSpecified)
            {
                ShowCommandHelp();
            }
            else
            {
                using (ReportingService2010 rs = new ReportingService2010())
                {
                    rs.Credentials = CredentialCache.DefaultCredentials;
                    rs.Url = this.Url;
                    _service = rs;
                    ExecuteOverride();
                }
            }
        }
        protected abstract void ExecuteOverride();
        protected virtual void ShowCommandHelp()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Usage");
            Console.WriteLine("-----");
            CommandAttribute attribute = this.GetType().GetCustomAttribute<CommandAttribute>();

            Console.Write($"RSService {attribute.Name} ");
            foreach (CommandArgumentMetadata metadata in _metadata.Values.OrderBy(t => t.Name))
            {
                if (metadata.Attributes.Optional)
                {
                    if (metadata.Attributes.HasValue)
                        Console.Write($"[-{metadata.Name} value]");
                    else
                        Console.Write($"[-{metadata.Name}]");
                }
                else
                {
                    if (metadata.Attributes.HasValue)
                        Console.Write($"-{metadata.Name} value");
                    else
                        Console.Write($"-{metadata.Name}");
                }
                Console.Write(" ");
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}
