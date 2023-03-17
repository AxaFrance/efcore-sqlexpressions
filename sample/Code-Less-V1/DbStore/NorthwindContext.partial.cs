using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Code_Less.DbStore;

public partial class NorthwindContext
{
    [DbFunction(Name = "SOUNDEX", IsBuiltIn = true)]
    public virtual string Soundex(string value)
    {
        throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(this.Soundex)));
    }
}