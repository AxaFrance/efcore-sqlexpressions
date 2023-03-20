using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Code_Flexibility.DbStore;

public static class DbFunctionExtensions
{
    internal static Type DeclaringType { get; } = typeof(DbFunctionExtensions);

    public static MethodInfo SoundexMethodInfo { get; } =
        DeclaringType.GetMethod(nameof(Soundex), new[] { typeof(DbFunctions), typeof(string) })!;

    public static string Soundex(this DbFunctions _, string value)
    {
        throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Soundex)));
    }
}