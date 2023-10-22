namespace Cassandra.Fluent.Migrator.Example
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config
                                .AddJsonFile("appsettings.json", false, true)
                                .AddJsonFile("appsettings.local.json", true, true);
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    })
                    .UseSerilog((hostingContext, loggerConfiguration) =>
                    {
                        loggerConfiguration
                                .ReadFrom
                                .Configuration(hostingContext.Configuration)
                                .Enrich
                                .FromLogContext();
                    });
        }
    }
}