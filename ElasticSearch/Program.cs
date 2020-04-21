namespace ElasticSearch
{
    using System.IO;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using NLog;
    using NLog.Extensions.Logging;

    using LogLevel = Microsoft.Extensions.Logging.LogLevel;

    public class Program
    {
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration(AppConfig).ConfigureLogging(LoggingConfig).UseStartup<Startup>();

        public static void Main(string[] args)
        {
            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segfault on Linux)
                LogManager.Shutdown();
            }
        }

        private static void AppConfig(WebHostBuilderContext context, IConfigurationBuilder config)
        {
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true);
            config.AddJsonFile("DevelopmentCredentials.json", optional: true, reloadOnChange: true);

            // Add environment variables after adding appsettings to override common variables.
            config.AddEnvironmentVariables();
        }

        private static void LoggingConfig(WebHostBuilderContext context, ILoggingBuilder loggingBuilder)
        {
            // clear all previously registered providers in CreateDefaultBuilder.
            loggingBuilder.ClearProviders();

            loggingBuilder.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            loggingBuilder.SetMinimumLevel(LogLevel.Trace);
        }
    }
}