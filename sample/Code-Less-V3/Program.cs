using Code_Less;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var hostBuilder = new HostBuilder();
        await hostBuilder.Configure()
            .RegisterServices(args)
            .StartAsync();
    }
}