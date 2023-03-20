using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace AxaFrance.EFCore.SqlExpressions.InMemory.Tests;

public static class CustomSqlDbFunctionsInMemoryExtensions
{
    private static Type DeclaringType { get; } = typeof(CustomSqlDbFunctionsInMemoryExtensions);

    public static MethodInfo MyCustomMethodInfo { get; } =
        DeclaringType.GetMethod(nameof(MyCustom), new[] { typeof(DbFunctions), typeof(int) })!;

    public static string MyCustom(this DbFunctions _, int value)
    {
        return $"=> {value}";
    }
}