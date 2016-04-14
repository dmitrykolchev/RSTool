using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSServices.RS2010;


namespace RSServices
{
    class Program
    {
        static void Main(string[] args)
        {
            ReportingService2010SoapClient client = new ReportingService2010SoapClient();
            client.Open();
            
        }
    }
}
