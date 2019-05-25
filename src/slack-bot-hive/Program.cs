using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace slack_bot_hive
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Initial logger which we'll override in Startup.cs
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {NewLine:l}{Message:lj}{NewLine}{Exception}{NewLine:l}")
                .CreateLogger();

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Log.ForContext<Program>().Warning("Application shutting down");
            };

            try
            {
                Log.Information("Starting web host");

                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
