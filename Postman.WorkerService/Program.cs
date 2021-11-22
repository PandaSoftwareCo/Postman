using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Postman.WorkerService
{
    public class Program
    {
        //public static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    if (hostContext.HostingEnvironment.IsDevelopment())
                    {

                    }

                    services.AddSingleton<IConfiguration>(provider => hostContext.Configuration);

                    services.AddHostedService<Worker>();
                }).ConfigureAppConfiguration(config =>
                {
                    config.AddInMemoryCollection(new[]
                    {
                        new KeyValuePair<string, string>("Key", "Value")
                    });
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddUserSecrets<Program>();
                }).ConfigureLogging(logging =>
                {
                    //logging.AddConfiguration()
                    //builder.SetMinimumLevel(LogLevel.Trace);
                    //logging.AddConfiguration(hostContext.Configuration);
                    logging.AddDebug();
                    //builder.AddConsole();
                    //builder.AddEventLog();
                    //builder.AddEventSourceLogger();
                    //builder.AddProvider();
                    //builder.AddTraceSource();

                });
    }
}
