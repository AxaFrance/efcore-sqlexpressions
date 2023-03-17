using AxaFrance.EFCore.SqlExpressions;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore;

public static class SqlDbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseAddedExpressions(
        this IRelationalDbContextOptionsBuilderInfrastructure builderInfrastructure)
    {
        return UseAddedExpressions(builderInfrastructure, default!);
    }

    public static DbContextOptionsBuilder UseAddedExpressions(
        this IRelationalDbContextOptionsBuilderInfrastructure builderInfrastructure,
        Action<Action<SqlExpressionConfiguration>> configure)
    {
        var optionsBuilder = builderInfrastructure.OptionsBuilder;
        var extension = optionsBuilder.Options.FindExtension<DbContextOptionsExtension>()
                        ?? new DbContextOptionsExtension();
        var addingSqlFunction = new List<SqlExpressionConfiguration>();
        configure?.Invoke(addingSqlFunction.Add);
        foreach (var (declaringType, methodInfo, createExpression) in addingSqlFunction)
        {
            extension.AddSqlFunction(declaringType, methodInfo, createExpression);
        }

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
        return optionsBuilder;
    }
}