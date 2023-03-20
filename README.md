# EntityFrameworkCore.SqlExpressions

EntityFrameworkCore.SqlExpressions permet d'enregistrer des SqlExpressions simplement dans le mecanisme de rendu d'
EntityFramework

## EntityFrameworkCore.SqlExpressions.InMemory

Il est possible de faire simplement vos tests d'integration via cette librairie [InMemory](docs/InMemory.md)

## Usage

### Configuration

```csharp
        
        services.AddDbContextPool<NorthwindContext>(builder =>
            builder.UseSqlServer("Data Source=localhost;Initial Catalog=Northwind;",
                optionsBuilder =>
                {
                    optionsBuilder.UseAddedExpressions(add =>
                    {
                        add(new SqlExpressionConfiguration(CustomSqlDbFunctionsExtensions.DeclaringType,
                            CustomSqlDbFunctionsExtensions.MyCustomMethodInfo,
                            arguments => new MyCustomSqlExpression(arguments)));
                    });
                }));
```

### DbFunctionExtensions

```csharp
public static class CustomSqlDbFunctionsExtensions
{
    public static Type DeclaringType { get; } = typeof(CustomSqlDbFunctionsExtensions);

    public static MethodInfo MyCustomMethodInfo { get; } =
        DeclaringType.GetMethod(nameof(MyCustom), new[] { typeof(DbFunctions), typeof(int) })!;

    public static string MyCustom(this DbFunctions _, int value) =>
        throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(MyCustom)));
}
```

### Custom SqlExpression

```csharp
public sealed class MyCustomSqlExpression : SqlExpression
{
    private readonly SqlExpression[] sqlExpressions;

    private static readonly StringTypeMapping relationalTypeMapping;
    
    private static readonly SqlConstantExpression constantExpression;
    
    private static readonly Type stringType;

    public MyCustomSqlExpression(SqlExpression[] sqlExpressions) : base(stringType, relationalTypeMapping)
    {
        this.sqlExpressions = sqlExpressions.Prepend(constantExpression).ToArray();
    }

    static MyCustomSqlExpression()
    {
        relationalTypeMapping = new("nvarchar(max)", DbType.String);
        stringType = typeof(string);
        constantExpression = new SqlConstantExpression(Constant("-> ", stringType), relationalTypeMapping);
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
```

### Query

```csharp
var result = dbContext.Regions.Select(r => EF.Functions.MyCustom(r.RegionId)).ToList();
```

### [License (MIT)](LICENCE)