using Code_Flexibility.DbStore.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Code_Less.DbStore;

public partial class NorthwindContext
{
    public virtual string Soundex(string value)
    {
        throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(this.Soundex)));
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(typeof(NorthwindContext).GetMethod(nameof(this.Soundex))!)
            .HasTranslation(arguments => new SoundexSqlFunctionExpression(arguments.ToArray()));
    }
}