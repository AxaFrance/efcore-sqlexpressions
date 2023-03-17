using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EntityFrameworkCore.SqlExpressions.Tests;

public static class CustomSqlDbFunctionsExtensions
{
    public static Type DeclaringType { get; } = typeof(CustomSqlDbFunctionsExtensions);

    public static MethodInfo MyCustomMethodInfo { get; } =
        DeclaringType.GetMethod(nameof(MyCustom), new[] { typeof(DbFunctions), typeof(int) })!;

    public static string MyCustom(this DbFunctions _, int value)
    {
        throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(MyCustom)));
    }
}