using System.Linq.Expressions;
using System.Reflection;

namespace AxaFrance.EFCore.SqlExpressions.InMemory.Query.Internal;

internal class InMemoryExpressionTranslatingExpressionVisitor : ExpressionVisitor
{
    private readonly IDictionary<MethodInfo, MethodInfo> translateMethodInfoByOriginMethodInfo;

    public InMemoryExpressionTranslatingExpressionVisitor(
        IDictionary<MethodInfo, MethodInfo> translateMethodInfoByOriginMethodInfo)
    {
        this.translateMethodInfoByOriginMethodInfo = translateMethodInfoByOriginMethodInfo;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        return this.translateMethodInfoByOriginMethodInfo.TryGetValue(node.Method, out var translate)
            ? Expression.Call(translate, node.Arguments)
            : base.VisitMethodCall(node);
    }
}