using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Json;

namespace Experimentation.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Information("Firing up the experimentation api...");

                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Prod";
                Log.Information($"Detected environment is: {env}");

                var envFilePath = $"appsettings.{env}";

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{envFilePath}.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();

                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Debug()
                    .WriteTo.RollingFile(new JsonFormatter(), "Logs/log-{Date}.json")
                    .CreateLogger();

                Log.Information("Firing up the experimentation api...");
                BuildWebHost(args, builder).Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Api  terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHost BuildWebHost(string[] args, IConfiguration builder) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                //.UseUrls("http://localhost:811")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .UseConfiguration(builder)
                .Build();
    }
}
