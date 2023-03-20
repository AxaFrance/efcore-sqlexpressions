using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

// ReSharper disable once CheckNamespace
namespace Code_Flexibility.DbStore.Expressions;

public sealed class SoundexSqlFunctionExpression : SqlFunctionExpression
{
    public SoundexSqlFunctionExpression(params SqlExpression[] arguments)
        : base("SOUNDEX", arguments, false, arguments.Select(a => false), typeof(string), null)
    {
    }
}