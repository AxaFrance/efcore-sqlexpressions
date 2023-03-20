using System.Diagnostics;
using Code_Flexibility.DbStore;
using EntityFrameworkCore.SqlExpressions.Tests;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace AxaFrance.EFCore.SqlExpressions.Tests;

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