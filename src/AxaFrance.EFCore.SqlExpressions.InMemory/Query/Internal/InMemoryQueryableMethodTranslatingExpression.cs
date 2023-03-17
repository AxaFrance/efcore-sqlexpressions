using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.InMemory.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;

namespace AxaFrance.EFCore.SqlExpressions.InMemory.Query.Internal;

#pragma warning disable EF1001
internal class InMemoryQueryableMethodTranslatingExpression : InMemoryQueryableMethodTranslatingExpressionVisitor
#pragma warning restore EF1001
{
    private readonly InMemoryExpressionTranslatingExpressionVisitor inMemoryExpressionTranslatingExpressionVisitor;

    public InMemoryQueryableMethodTranslatingExpression(
        QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
        QueryCompilationContext queryCompilationContext,
        IDictionary<MethodInfo, MethodInfo> translateMethodInfoByOriginMethodInfo)
#pragma warning disable EF1001
        : base(dependencies, queryCompilationContext)
#pragma warning restore EF1001
    {
        this.inMemoryExpressionTranslatingExpressionVisitor =
            new InMemoryExpressionTranslatingExpressionVisitor(translateMethodInfoByOriginMethodInfo);
    }

    public override Expression? Visit(Expression? node)
    {
        var changedExpression = this.inMemoryExpressionTranslatingExpressionVisitor.Visit(node);
        return base.Visit(changedExpression);
    }
}