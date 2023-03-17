using Code_Less.DbStore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Code_Less.HostedService;

public sealed class SampleHostedService : BackgroundService
{
    private readonly ILogger<SampleHostedService> logger;
    private readonly NorthwindContext northwindContext;

    public SampleHostedService(ILogger<SampleHostedService> logger, NorthwindContext northwindContext)
    {
        this.logger = logger;
        this.northwindContext = northwindContext;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var filteredTerritories = this.northwindContext.Territories
            .Where(t => this.northwindContext.Soundex(t.TerritoryDescription) == this.northwindContext.Soundex("senta"))
            .Select(t => t.TerritoryDescription)
            .ToList();
        filteredTerritories.ForEach(territory => this.logger.LogInformation(territory));

        return Task.CompletedTask;
    }
}