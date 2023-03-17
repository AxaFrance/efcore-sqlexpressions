using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SqlExpressions.Tests;

public sealed class MyCustomSqlExpression : SqlExpression
{
    private static readonly StringTypeMapping relationalTypeMapping;

    private static readonly SqlConstantExpression constantExpression;

    private static readonly Type stringType;
    private readonly SqlExpression[] sqlExpressions;

    static MyCustomSqlExpression()
    {
        relationalTypeMapping = new StringTypeMapping("nvarchar(max)", DbType.String);
        stringType = typeof(string);
        constantExpression = new SqlConstantExpression(Constant("-> ", stringType), relationalTypeMapping);
    }

    public MyCustomSqlExpression(SqlExpression[] sqlExpressions) : base(stringType, relationalTypeMapping)
    {
        this.sqlExpressions = sqlExpressions.Prepend(constantExpression).ToArray();
    }

    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        return new SqlFunctionExpression("CONCAT", this.sqlExpressions, false,
            this.sqlExpressions.Select(_ => false), stringType,
            this.TypeMapping);
    }

    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        expressionPrinter.Visit(this);
    }
}