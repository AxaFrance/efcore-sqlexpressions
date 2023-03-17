using Code_Less.DbStore;
using Code_Less.HostedService;
using Faker;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;

namespace Code_Less.Tests;

[TestFixture]
public class SampleHostedServiceShould
{
    [SetUp]
    public async Task BeforeEach()
    {
        this.mockedDbContext =
            new Mock<NorthwindContext>(() => new NorthwindContext(new DbContextOptions<NorthwindContext>()));
        this.provider = new ServiceCollection()
            .AddScoped<Mock<NorthwindContext>>(_ => this.mockedDbContext)
            .AddScoped<NorthwindContext>(_ => this.mockedDbContext.Object)
            .AddScoped(_ => this.loggerMock.Object)
            .AddHostedService<SampleHostedService>()
            .BuildServiceProvider();
        this.mockedDbContext = this.provider.GetRequiredService<Mock<NorthwindContext>>();
    }

    [TearDown]
    public async Task AfterEach()
    {
        await this.provider!.DisposeAsync();
    }

    private readonly Mock<ILogger<SampleHostedService>> loggerMock = new();
    private ServiceProvider provider;
    private Mock<NorthwindContext> mockedDbContext;

    private void PopulateDbSet(params Territory[] territories)
    {
        for (var index = 0; index < territories.Length; index++)
        {
            var territory = territories[index];
            territory.TerritoryId ??= $"primarykey {index}";
        }

        this.mockedDbContext.Setup(c => c.Territories).ReturnsDbSet(territories);
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
        this.mockedDbContext.Setup(m => m.Soundex("senta")).Returns("S530");
        this.mockedDbContext.Setup(m => m.Soundex(It.Is<string>(v => v.StartsWith("Samta ")))).Returns("S530");
        this.mockedDbContext.Setup(m => m.Soundex(It.Is<string>(v => v.StartsWith("Santa ")))).Returns("S530");
        this.mockedDbContext.Setup(m => m.Soundex(It.Is<string>(v => v.StartsWith("Semta ")))).Returns("S530");
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