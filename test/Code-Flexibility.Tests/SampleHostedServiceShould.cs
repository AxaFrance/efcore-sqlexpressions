using Code_Flexibility.DbStore;
using Code_Flexibility.HostedService;
using Faker;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace Code_Flexibility.Tests;

[TestFixture]
public class SampleHostedServiceShould
{
    [SetUp]
    public async Task BeforeEach()
    {
        this.provider = new ServiceCollection()
            .AddDbContextPool<NorthwindContext>(builder =>
                builder.UseInMemoryDatabase("Northwind", optionsBuilder => optionsBuilder
                    .UseAddedExpressions()))
            .AddScoped(_ => this.loggerMock.Object)
            .AddHostedService<SampleHostedService>()
            .BuildServiceProvider();
        this.dbContext = this.provider.GetRequiredService<NorthwindContext>();
        this.dbContext.Database.EnsureDeleted();
        await this.dbContext.SaveChangesAsync();
    }

    [TearDown]
    public async Task AfterEach()
    {
        await this.dbContext!.DisposeAsync();
        await this.provider!.DisposeAsync();
    }

    private readonly Mock<ILogger<SampleHostedService>> loggerMock = new();
    private ServiceProvider provider;
    private NorthwindContext dbContext;

    private void PopulateDbSet(params Territory[] territories)
    {
        for (var index = 0; index < territories.Length; index++)
        {
            var territory = territories[index];
            territory.TerritoryId ??= $"primarykey {index}";
        }

        this.dbContext.Territories.AddRange(territories);
        this.dbContext.SaveChanges();
    }

    [Test]
    public async Task LogInInformationTheFilteredTerritoriesWichApprochingSentaKeyWord()
    {
        var territories = Builder<Territory>.CreateListOfSize(20)
            .All()
            .With(t => t.TerritoryDescription = Address.UsTerritory())
            .TheFirst(3)
            .With((territory, index) => territory.TerritoryDescription = $"Samta {index}")
            .TheNext(2)
            .With((territory, index) => territory.TerritoryDescription = $"Santa {index}")
            .TheLast(3)
            .With((territory, index) => territory.TerritoryDescription = $"Semta {index}")
            .Build()
            .ToArray();
        var expectedTerritories = territories.Where(t =>
                t.TerritoryDescription.StartsWith("Samta")
                || t.TerritoryDescription.StartsWith("Santa")
                || t.TerritoryDescription.StartsWith("Semta"))
            .ToList();
        this.PopulateDbSet(territories);
        var service = this.provider.GetRequiredService<IHostedService>();

        await service.StartAsync(CancellationToken.None);

        expectedTerritories.Should().NotBeEmpty();
        expectedTerritories.ForEach(territory =>
        {
            this.loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>(
                        (v, _) => v.ToString() == territory.TerritoryDescription),
                    It.IsAny<Exception>(),
                    ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
                Times.Exactly(1));
        });

        this.loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
            Times.Exactly(expectedTerritories.Count));
    }
}