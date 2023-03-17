using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AxaFrance.EFCore.SqlExpressions;

public static class SqlDbFunctionsExtensions
{
    internal static Type DeclaringType { get; } = typeof(SqlDbFunctionsExtensions);

    public static MethodInfo SoundexMethodInfo { get; } =
        DeclaringType.GetMethod(nameof(Soundex), new[] { typeof(DbFunctions), typeof(string) })!;

    public static string Soundex(this DbFunctions _, string value)
    {
        throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Soundex)));
    }
}