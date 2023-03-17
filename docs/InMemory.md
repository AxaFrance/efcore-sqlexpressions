# EntityFrameworkCore.SqlExpressions.InMemory

EntityFrameworkCore.SqlExpressions.InMemory permet d'enregistrer vos methode correspondant a vos SqlExpressions
simplement dans le mecanisme de generation d'EntityFramework.
Ainsi vous pouvez faire vos test d'integration au sein de vos projets.

## Usage

### Configuration

```csharp
        
        services.AddDbContextPool<NorthwindContext>(builder =>
           builder.UseInMemoryDatabase("Northwind", 
                optionsBuilder => optionsBuilder.UseAddedExpressions(add =>
                        add(new MethodInfoTranslatorConfiguration(
                            CustomSqlDbFunctionsExtensions.MyCustomMethodInfo,
                            CustomSqlDbFunctionsInMemoryExtensions.MyCustomMethodInfo)))))
```

### DbFunctionExtensions

```csharp
public static class CustomSqlDbFunctionsInMemoryExtensions
{
    public static Type DeclaringType { get; } = typeof(CustomSqlDbFunctionsInMemoryExtensions);

    public static MethodInfo MyCustomMethodInfo { get; } =
        DeclaringType.GetMethod(nameof(MyCustom), new[] { typeof(DbFunctions), typeof(int) })!;

    public static string MyCustom(this DbFunctions _, int value) => $"-> {value}";
}
```

### Query

```csharp
//TODO: return list strings. value of item is "-> regionId".
var result = dbContext.Regions.Select(r => EF.Functions.MyCustom(r.RegionId)).ToList();
```