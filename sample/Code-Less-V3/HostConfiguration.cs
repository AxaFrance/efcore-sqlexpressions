using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Code_Less
{
    internal static class HostConfiguration
    {
        public static IHostBuilder Configure(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureHostConfiguration(
                    builder => builder.AddEnvironmentVariables()
                        .AddJsonFile("appsettings.json", false))
                .ConfigureLogging(
                    (context, builder) =>
                    {
                        var configuration = context.Configuration;
                        ILogger logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(configuration)
                            .CreateLogger();
                        builder.AddConfiguration(configuration)
                            .AddSerilog(logger);
                    })
                .UseConsoleLifetime();
        }
    }
}