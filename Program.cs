using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

namespace MBCEventDonation
{
    public class Program
    {
        private static IConfiguration _config;

        public Program(IConfiguration configuration)
        {

            _config = configuration;
        }

        //public static void Main(string[] args)
        //{
        //    CreateHostBuilder(args).Build().Run();


        //}

        //https://nblumhardt.com/2019/10/serilog-in-aspnetcore-3/
        public static void Main(string[] args)
        {
            string fileName = "Errorlog.json";
            // string path = AppDomain.CurrentDomain.BaseDirectory;
            //string pathString = System.IO.Path.Combine(path, fileName);
            DateTime dt = DateTime.UtcNow;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                 // .WriteTo.RollingFile()
                 //.WriteTo.File(new RenderedCompactJsonFormatter(), pathString)
                 .WriteTo.File(new RenderedCompactJsonFormatter(), fileName, Serilog.Events.LogEventLevel.Information)
                .CreateLogger();
           

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
            
            //.ConfigureLogging((context, logging) =>
            //{
            //    logging.ClearProviders();
            //    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
            //    logging.AddDebug();
            //    logging.AddConsole();// eventsource, eventlog, , tracesource, azureappservicefile, azureallserviceblob, applicationinsight

            //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}
