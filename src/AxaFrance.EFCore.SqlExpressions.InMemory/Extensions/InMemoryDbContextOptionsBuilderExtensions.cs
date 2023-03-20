using AxaFrance.EFCore.SqlExpressions.InMemory.Extensions;
using AxaFrance.EFCore.SqlExpressions;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore;

public static class InMemoryDbContextOptionsBuilderExtensions
{
    public static InMemoryDbContextOptionsBuilder UseAddedExpressions(
        this InMemoryDbContextOptionsBuilder builderInfrastructure,
        Action<Action<MethodInfoTranslatorConfiguration>> configure)
    {
        var optionsBuilder = ((IInMemoryDbContextOptionsBuilderInfrastructure)builderInfrastructure).OptionsBuilder;
        optionsBuilder.EnableServiceProviderCaching(false);
        var extension = optionsBuilder.Options.FindExtension<DbContextInMemoryOptionsExtension>()
                        ?? new DbContextInMemoryOptionsExtension();
        var translateMethodInfoByOriginMethodInfo = new List<MethodInfoTranslatorConfiguration>();
        configure?.Invoke(translateMethodInfoByOriginMethodInfo.Add);
        foreach (var (origin, inMemory) in translateMethodInfoByOriginMethodInfo)
        {
            extension.AddOriginDeclarationTranslate(origin, inMemory);
        }

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
        return builderInfrastructure;
    }
}