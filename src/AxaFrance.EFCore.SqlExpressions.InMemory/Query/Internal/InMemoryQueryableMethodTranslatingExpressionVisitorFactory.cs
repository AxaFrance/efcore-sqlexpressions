using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;

namespace AxaFrance.EFCore.SqlExpressions.InMemory.Query.Internal;

public sealed class
    InMemoryQueryableMethodTranslatingExpressionVisitorFactory : IQueryableMethodTranslatingExpressionVisitorFactory
{
    private readonly QueryableMethodTranslatingExpressionVisitorDependencies dependencies;
    private readonly IDictionary<MethodInfo, MethodInfo> translateMethodInfoByOriginMethodInfo;

    public InMemoryQueryableMethodTranslatingExpressionVisitorFactory(
        QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
        IDictionary<MethodInfo, MethodInfo> translateMethodInfoByOriginMethodInfo)
    {
        this.dependencies = dependencies;
        this.translateMethodInfoByOriginMethodInfo = translateMethodInfoByOriginMethodInfo;
    }

    public QueryableMethodTranslatingExpressionVisitor Create(QueryCompilationContext queryCompilationContext)
    {
        return new InMemoryQueryableMethodTranslatingExpression(this.dependencies, queryCompilationContext,
            this.translateMethodInfoByOriginMethodInfo);
    }
}