using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace AxaFrance.EFCore.SqlExpressions.Query.Internal;

public class MethodCallTranslator : IMethodCallTranslator
{
    private static readonly Type dbFunctionsType = typeof(DbFunctions);
    private readonly CreateExpression createExpression;
    private readonly MethodInfo methodInfo;
    private readonly IRelationalTypeMappingSource typeMappingSource;

    public MethodCallTranslator(IRelationalTypeMappingSource typeMappingSource, MethodInfo methodInfo,
        CreateExpression createExpression)
    {
        this.typeMappingSource = typeMappingSource;
        this.methodInfo = methodInfo;
        this.createExpression = createExpression ?? throw new ArgumentNullException(nameof(createExpression));
    }

    public SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        if (method != this.methodInfo)
        {
            return null;
        }

        var expressions = method.GetParameters()
            .Select((parameterInfo, index) => (parameterInfo.ParameterType, index))
            .Where(item => item.ParameterType != dbFunctionsType)
            .Select(item =>
            {
                var expression = arguments[item.index];
                if (expression is not SqlConstantExpression sqlConstantExpression)
                {
                    return expression;
                }

                var typeMapping = this.typeMappingSource.FindMapping(item.ParameterType);
                return sqlConstantExpression.ApplyTypeMapping(typeMapping);
            })
            .ToArray();

        return this.createExpression(expressions);
    }
}

public delegate SqlExpression CreateExpression(params SqlExpression[] arguments);