using System.Reflection;

namespace AxaFrance.EFCore.SqlExpressions.InMemory.Extensions;

public sealed class MethodInfoTranslatorConfiguration
{
    public MethodInfoTranslatorConfiguration(MethodInfo origin, MethodInfo inMemory)
    {
        this.Origin = origin;
        this.InMemory = inMemory;
    }

    public MethodInfo Origin { get; }
    public MethodInfo InMemory { get; }

    public void Deconstruct(out MethodInfo origin, out MethodInfo inMemory)
    {
        origin = this.Origin;
        inMemory = this.InMemory;
    }
}