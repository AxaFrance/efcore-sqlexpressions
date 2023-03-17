using Microsoft.EntityFrameworkCore.Query;

namespace AxaFrance.EFCore.SqlExpressions.Query.Internal;

internal sealed class MethodCallTranslatorPlugin : IMethodCallTranslatorPlugin
{
    public MethodCallTranslatorPlugin(IMethodCallTranslator[] methodCallTranslators)
    {
        this.Translators = methodCallTranslators;
    }

    public IEnumerable<IMethodCallTranslator> Translators { get; }
}