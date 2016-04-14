using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSServices.RS2010;
using System.Net;

namespace RSServices
{
    public abstract class CommandBase : Command
    {
        protected CommandBase()
        {
        }
        [CommandArgument(HasValue = true)]
        public string Url { get; set; }
        public bool IsUrlSpecified { get; set; }
        private ReportingService2010 _service;
        public ReportingService2010 Service => _service;
        internal override void ExecuteInternal(CommandExecutionContext context)
        {
            using (ReportingService2010 rs = new ReportingService2010())
            {
                rs.Credentials = CredentialCache.DefaultCredentials;
                rs.Url = this.Url;
                _service = rs;
                base.ExecuteInternal(context);
            }
        }
    }
}
