using System.Diagnostics;
using AxaFrance.EFCore.SqlExpressions;
using Code_Flexibility.DbStore;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.SqlExpressions.Tests;

[TestFixture]
public class DbContextOptionBuilderExtensionsShould
{
    [OneTimeSetUp]
    public void StartTest()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    [OneTimeTearDown]
    public void EndTest()
    {
        Trace.Flush();
    }

    [Test]
    public async Task GenerateSqlQueryWithSoudexExpression()
    {
        await using var provider = new ServiceCollection()
            .AddDbContextPool<NorthwindContext>(builder =>
                builder.UseSqlServer("Data Source=fakeDataSource;Initial Catalog=FakeCatalog;",
                    optionsBuilder => optionsBuilder.UseAddedExpressions()))
            .BuildServiceProvider();
        await using var dbContext = provider.GetRequiredService<NorthwindContext>();

        var query = dbContext.Regions.Where(r =>
            EF.Functions.Soundex(r.RegionDescription) == EF.Functions.Soundex("Haut de France"));

        query.As<EntityQueryable<Region>>().DebugView.Query.Replace(Environment.NewLine, " ")
            .Should()
            .Be(
                "SELECT [r].[RegionID], [r].[RegionDescription] FROM [Region] AS [r] WHERE SOUNDEX([r].[RegionDescription]) = SOUNDEX(N'Haut de France')");
    }

    [Test]
    public async Task GenerateSqlQueryWithCustomSqlExpression()
    {
        var services = new ServiceCollection();
        services.AddDbContextPool<NorthwindContext>(builder =>
            builder.UseSqlServer("Data Source=fakeDataSource;Initial Catalog=fakeCatalog;",
                optionsBuilder =>
                {
                    optionsBuilder.UseAddedExpressions(add =>
                    {
                        add(new SqlExpressionConfiguration(CustomSqlDbFunctionsExtensions.DeclaringType,
                            CustomSqlDbFunctionsExtensions.MyCustomMethodInfo,
                            arguments => new MyCustomSqlExpression(arguments)));
                    });
                }));
        await using var provider = services.BuildServiceProvider();
        await using var dbContext = provider.GetRequiredService<NorthwindContext>();

        var query = dbContext.Regions.Select(r => EF.Functions.MyCustom(r.RegionId));

        query.As<EntityQueryable<string>>().DebugView.Query.Replace(Environment.NewLine, " ")
            .Should()
            .Be("SELECT CONCAT('-> ', [r].[RegionID]) FROM [Region] AS [r]");
    }
}