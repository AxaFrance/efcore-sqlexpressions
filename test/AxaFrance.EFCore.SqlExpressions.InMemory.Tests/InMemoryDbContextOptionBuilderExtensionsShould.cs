using AxaFrance.EFCore.SqlExpressions.InMemory.Extensions;
using Code_Flexibility.DbStore;
using EntityFrameworkCore.SqlExpressions.Tests;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.SqlExpressions.InMemory.Tests;

[TestFixture]
public class InMemoryDbContextOptionBuilderExtensionsShould
{
    [Test]
    public async Task AddCustomSqlExpressionIntoProvider()
    {
        await using var provider = new ServiceCollection()
            .AddDbContextPool<NorthwindContext>(builder =>
                builder.UseInMemoryDatabase("Northwind", optionsBuilder => optionsBuilder
                    .UseAddedExpressions(add =>
                        add(new MethodInfoTranslatorConfiguration(
                            CustomSqlDbFunctionsExtensions.MyCustomMethodInfo,
                            CustomSqlDbFunctionsInMemoryExtensions.MyCustomMethodInfo)))))
            .BuildServiceProvider();
        await using var dbContext = provider.GetRequiredService<NorthwindContext>();
        var region = new Region
        {
            RegionId = 56,
            RegionDescription = "whatever"
        };
        await dbContext.Regions.AddAsync(region);
        await dbContext.SaveChangesAsync();

        var actualValue = dbContext.Regions.Select(r => EF.Functions.MyCustom(r.RegionId)).ToList();

        actualValue.Should()
            .ContainSingle()
            .Subject
            .Should()
            .Be("=> 56");
    }
}