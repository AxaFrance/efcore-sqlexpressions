using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

// ReSharper disable once CheckNamespace
namespace AxaFrance.EFCore.SqlExpressions.Query;

public sealed class SoundexSqlFunctionExpression : SqlFunctionExpression
{
    public SoundexSqlFunctionExpression(params SqlExpression[] arguments)
        : base("SOUNDEX", arguments, false, arguments.Select(a => false), typeof(string), null)
    {
    }
}