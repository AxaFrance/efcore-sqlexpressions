using System.Reflection;
using AxaFrance.EFCore.SqlExpressions.Query.Internal;

namespace AxaFrance.EFCore.SqlExpressions;

public sealed class SqlExpressionConfiguration
{
    public SqlExpressionConfiguration(Type declaringType, MethodInfo methodInfo, CreateExpression createExpression)
    {
        this.DeclaringType = declaringType;
        this.MethodInfo = methodInfo;
        this.CreateExpression = createExpression;
    }

    public Type DeclaringType { get; }
    public MethodInfo MethodInfo { get; }
    public CreateExpression CreateExpression { get; }

    public void Deconstruct(out Type declaringType, out MethodInfo methodInfo, out CreateExpression createExpression)
    {
        declaringType = this.DeclaringType;
        methodInfo = this.MethodInfo;
        createExpression = this.CreateExpression;
    }
}