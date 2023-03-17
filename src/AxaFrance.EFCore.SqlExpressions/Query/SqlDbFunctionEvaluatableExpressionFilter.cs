using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace AxaFrance.EFCore.SqlExpressions.Query;

internal sealed class SqlDbFunctionEvaluatableExpressionFilter : EvaluatableExpressionFilter
{
    private readonly Type[] declaringTypes;

    internal SqlDbFunctionEvaluatableExpressionFilter(EvaluatableExpressionFilterDependencies dependencies,
        Type[] declaringTypes)
        : base(dependencies)
    {
        this.declaringTypes = declaringTypes;
    }

    public override bool IsEvaluatableExpression(Expression expression, IModel model)
    {
        var isEvaluatableExpression = base.IsEvaluatableExpression(expression, model);
        if (expression is MethodCallExpression methodCallExpression &&
            this.declaringTypes.Contains(methodCallExpression.Method.DeclaringType))
        {
            return false;
        }

        return isEvaluatableExpression;
    }
}