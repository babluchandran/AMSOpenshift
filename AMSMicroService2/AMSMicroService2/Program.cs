using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Timers;
using AMSMicroService2.Controllers;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using AMSMicroService2.Models;

namespace AMSMicroService2
{
    public class Program
    {
        //private static System.Timers.Timer aTimer;
        public static void Main(string[] args)
        {
            IConnectionFactory factory = new ConnectionFactory("activemq:tcp://localhost:61616");
            
            IConnection _connection = factory.CreateConnection();
            _connection.Start();
            ISession _session = _connection.CreateSession();

            IDestination dest1 = _session.GetQueue("OPLDProcess");
            IMessageConsumer consumer = _session.CreateConsumer(dest1);
            consumer.Listener += Consumer_Listener; 

            CreateWebHostBuilder(args).Build().Run();
        }

        private static void Consumer_Listener(IMessage message)
        {
             // Read from MQ
            OPLD opldObject = Newtonsoft.Json.JsonConvert.DeserializeObject<OPLD>((message as ITextMessage).Text);
            CreateServicePointController createServicePointController = new CreateServicePointController();
            createServicePointController.MicroServiceServicePointGeneration(opldObject);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
