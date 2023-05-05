# EntityFrameworkCore.SqlExpressions
[![Continuous Integration](https://github.com/AxaFrance/efcore-sqlexpressions/actions/workflows/efcore-sqlexpressions.yml/badge.svg)](https://github.com/AxaFrance/efcore-sqlexpressions/actions/workflows/efcore-sqlexpressions.yaml) [![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=AxaFrance_efcore-sqlexpressions&metric=alert_status)](https://sonarcloud.io/dashboard?id=AxaFrance_efcore-sqlexpressions) [![Reliability](https://sonarcloud.io/api/project_badges/measure?project=AxaFrance_efcore-sqlexpressions&metric=reliability_rating)](https://sonarcloud.io/component_measures?id=AxaFrance_efcore-sqlexpressions&metric=reliability_rating) [![Security](https://sonarcloud.io/api/project_badges/measure?project=AxaFrance_efcore-sqlexpressions&metric=security_rating)](https://sonarcloud.io/component_measures?id=AxaGuilDEv_ml-cli&metric=security_rating) [![Code Corevage](https://sonarcloud.io/api/project_badges/measure?project=AxaFrance_efcore-sqlexpressions&metric=coverage)](https://sonarcloud.io/component_measures?id=AxaFrance_efcore-sqlexpressions&metric=Coverage)  

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