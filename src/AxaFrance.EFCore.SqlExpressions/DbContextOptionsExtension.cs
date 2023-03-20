using System.Reflection;
using AxaFrance.EFCore.SqlExpressions.Query;
using AxaFrance.EFCore.SqlExpressions.Query.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace AxaFrance.EFCore.SqlExpressions;

internal sealed class DbContextOptionsExtension : IDbContextOptionsExtension
{
    private readonly Dictionary<Type, List<(MethodInfo methodInfo, CreateExpression factory)>> additionalSqlFunction =
        new();

    private DbContextOptionsExtensionInfo? info;

    public void ApplyServices(IServiceCollection services)
    {
        foreach (var (_, factories) in this.additionalSqlFunction)
        {
            factories.ForEach(sqlExpressionInfo =>
            {
                services.AddScoped<IMethodCallTranslator>(provider =>
                {
                    var typeMappingSource = provider.GetRequiredService<IRelationalTypeMappingSource>();
                    return new MethodCallTranslator(typeMappingSource, sqlExpressionInfo.methodInfo,
                        sqlExpressionInfo.factory);
                });
            });
        }

        services.AddScoped<IEvaluatableExpressionFilter>(provider =>
            {
                var dependencies = provider.GetRequiredService<EvaluatableExpressionFilterDependencies>();
                return new SqlDbFunctionEvaluatableExpressionFilter(dependencies,
                    this.additionalSqlFunction.Keys.ToArray());
            })
            .AddScoped<IMethodCallTranslatorPlugin, MethodCallTranslatorPlugin>()
            .AddScoped<IMethodCallTranslator[]>(provider => provider.GetServices<IMethodCallTranslator>().ToArray());
    }

    public void Validate(IDbContextOptions options)
    {
        // Method intentionally left empty.
    }

    public DbContextOptionsExtensionInfo Info => this.info ??= new ExtensionInfo(this,
        string.Join(", ",
            this.additionalSqlFunction.Values.SelectMany(list => list.Select(m => $"{m.methodInfo.Name}: Ok"))));

    internal void AddSqlFunction(Type declaringType,
        MethodInfo methodInfo, CreateExpression createExpression)
    {
        var createExpressions = new List<(MethodInfo methodInfo, CreateExpression factory)>();
        if (!this.additionalSqlFunction.ContainsKey(declaringType))
        {
            this.additionalSqlFunction[declaringType] = createExpressions;
        }

        createExpressions.Add((methodInfo, createExpression));
    }

    internal sealed class ExtensionInfo : DbContextOptionsExtensionInfo
    {
        public ExtensionInfo(IDbContextOptionsExtension extension, string logFragment)
            : base(extension)
        {
            this.LogFragment = logFragment;
        }

        public override bool IsDatabaseProvider => false;

        public override string LogFragment { get; }

        public override int GetServiceProviderHashCode()
        {
            var hashCode = new HashCode();
            return hashCode.ToHashCode();
        }

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
        {
            return false;
        }

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
        }
    }
}