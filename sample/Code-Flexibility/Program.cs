using Microsoft.Extensions.Hosting;

namespace Code_Flexibility;

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