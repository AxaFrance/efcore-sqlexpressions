using Code_Flexibility.DbStore;
using AxaFrance.EFCore.SqlExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Code_Flexibility.HostedService;

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
            .Where(t => EF.Functions.Soundex(t.TerritoryDescription) == EF.Functions.Soundex("senta"))
            .Select(t => t.TerritoryDescription)
            .ToList();
        filteredTerritories.ForEach(territory => this.logger.LogInformation(territory));

        return Task.CompletedTask;
    }
}