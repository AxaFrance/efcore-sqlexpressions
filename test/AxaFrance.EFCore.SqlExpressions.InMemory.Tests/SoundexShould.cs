using System.Globalization;
using AxaFrance.EFCore.SqlExpressions;
using Code_Flexibility.DbStore;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.SqlExpressions.InMemory.Tests;

[TestFixture]
public class SoundexShould
{
    [SetUp]
    public void BeforeEach()
    {
        this.provider = new ServiceCollection()
            .AddDbContextPool<NorthwindContext>(builder =>
                builder.UseInMemoryDatabase("Northwind", optionsBuilder => optionsBuilder
                    .UseAddedExpressions()))
            .BuildServiceProvider();
        this.dbContext = this.provider.GetRequiredService<NorthwindContext>();
        this.dbContext.Database.EnsureDeleted();
        this.dbContext.SaveChanges();
    }

    [TearDown]
    public void AfterEach()
    {
        this.dbContext!.Dispose();
        this.provider!.Dispose();
    }

    private ServiceProvider provider;
    private NorthwindContext dbContext;

    private void Populate(params Territory[] territories)
    {
        for (var index = 0; index < territories.Length; index++)
        {
            var territory = territories[index];
            territory.TerritoryId ??= $"primarykey {index}";
        }

        this.dbContext.Territories.AddRange(territories);
        this.dbContext.SaveChanges();
    }

    [TestCase("", "0000")]
    [TestCase("a", "A000")]
    [TestCase("B", "B000")]
    [TestCase("c", "C000")]
    [TestCase("D", "D000")]
    [TestCase("e", "E000")]
    [TestCase("F", "F000")]
    [TestCase("g", "G000")]
    [TestCase("H", "H000")]
    [TestCase("i", "I000")]
    [TestCase("J", "J000")]
    [TestCase("k", "K000")]
    [TestCase("L", "L000")]
    [TestCase("m", "M000")]
    [TestCase("N", "N000")]
    [TestCase("o", "O000")]
    [TestCase("P", "P000")]
    public void TheFirstLetterOfCodeIsTheFirstUppercaseLetterOfWord(string value, string code)
    {
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.StartWithEquivalentOf(value.ToUpper());
    }

    [TestCase("aA", "A000")]
    [TestCase("Be", "B000")]
    [TestCase("cH", "C000")]
    [TestCase("Di", "D000")]
    [TestCase("eO", "E000")]
    [TestCase("Fu", "F000")]
    [TestCase("gW", "G000")]
    [TestCase("Hy", "H000")]
    public void A_E_H_I_O_U_W_Y_AreNotTranslated(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-EN");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("aB", "A100")]
    [TestCase("Bf", "B100")]
    [TestCase("cP", "C100")]
    [TestCase("Dv", "D100")]
    public void B_F_P_V_AreTranslatedToOneInCodeForEnglishCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-EN");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("aC", "A200")]
    [TestCase("Bg", "B200")]
    [TestCase("cJ", "C200")]
    [TestCase("Dk", "D200")]
    [TestCase("aQ", "A200")]
    [TestCase("Bs", "B200")]
    [TestCase("cX", "C200")]
    [TestCase("Dz", "D200")]
    public void C_G_J_K_Q_S_X_Z_AreTranslatedToTwoInCodeForEnglishCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-EN");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("ad", "A300")]
    [TestCase("BT", "B300")]
    public void D_T_AreTranslatedToThreeInCodeForEnglishCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-EN");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("aL", "A400")]
    public void L_IsTranslatedToFourInCodeForEnglishCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-EN");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("aM", "A500")]
    [TestCase("an", "A500")]
    public void M_N_IsTranslatedToFiveInCodeForEnglishCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-EN");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("aB", "A100")]
    [TestCase("aP", "A100")]
    public void B_P_IsTranslatedToOneInCodeForFrenchCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("Bc", "B200")]
    [TestCase("cK", "C200")]
    [TestCase("Dq", "D200")]
    public void C_K_Q_AreTranslatedToTwoInCodeForFrenchCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("cD", "C300")]
    [TestCase("Dt", "D300")]
    public void D_T_AreTranslatedToThreeInCodeForFrenchCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("cL", "C400")]
    public void L_IsTranslatedToFourInCodeForFrenchCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("cM", "C500")]
    [TestCase("cn", "C500")]
    public void M_N_AreTranslatedToFiveInCodeForFrenchCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("cR", "C600")]
    public void R_AreTranslatedToSixInCodeForFrenchCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("cG", "C700")]
    [TestCase("cj", "C700")]
    public void G_J_AreTranslatedToSevenInCodeForFrenchCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("cx", "C800")]
    [TestCase("cZ", "C800")]
    [TestCase("cs", "C800")]
    public void X_Z_S_AreTranslatedToEightInCodeForFrenchCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("cf", "C900")]
    [TestCase("cV", "C900")]
    public void F_V_AreTranslatedToNineInCodeForFrenchCulture(string value, string code)
    {
        CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("cbBb", "C100")]
    [TestCase("cbpb", "C100")]
    public void NotTranslateSameLetterFollowingEachOther(string value, string code)
    {
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }

    [TestCase("cbcdl", "C123")]
    [TestCase("Redmond", "R355")]
    public void TheCodeHaveForCaracteresMax(string value, string code)
    {
        var territory = new Territory(value);
        this.Populate(territory);
        var actualValue = this.dbContext.Territories.Select(t => EF.Functions.Soundex(value)).Single();

        var assertions = actualValue.Should();
        assertions.Be(code);
    }
}