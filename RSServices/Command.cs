using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSServices.RS2010;
using System.Net;

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
        protected Command()
        {
        }
        public bool IsUrlSpecified { get; set; }
        public string Url { get; set; }
        public void Parse(string[] args)
        {
            for(int index = 1; index < args.Length; ++index)
            {
                ArgumentOptions options = ValidateCommandArgument(args[index].ToLower());
                if((options & ArgumentOptions.HasValue) != 0)
                {
                    SetArgumentValue(args[index].ToLower(), args[index + 1]);
                    index++;
                }
            }
            ValidateCommand();
        }
        protected virtual void ValidateCommand()
        {
            if (!IsUrlSpecified)
                throw new InvalidOperationException();
        }
        protected virtual ArgumentOptions ValidateCommandArgument(string name)
        {
            switch(name)
            {
                case "-url":
                    IsUrlSpecified = true;
                    return ArgumentOptions.HasValue;
                default:
                    throw new InvalidOperationException();                  
            }
        }
        protected virtual void SetArgumentValue(string name, string value)
        {
            switch (name)
            {
                case "-url":
                    Url = value;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
        private ReportingService2010 _service;

        public ReportingService2010 Service => _service;

        public void Execute()
        {
            using (ReportingService2010 rs = new ReportingService2010())
            {
                rs.Credentials = CredentialCache.DefaultCredentials;
                rs.Url = this.Url;
                _service = rs;
                ExecuteOverride();
            }
        }
        protected abstract void ExecuteOverride();
    }
}
