using Tavsan.Core.Domain;
using Tavsan.Core.Services;

namespace Tavsan.Tests;

public class ScenarioParserTests
{
    [Fact]
    public void Parses_sample_scenario()
    {
        var ok = ScenarioParser.TryParse("N,N,L,J,N,N,İ,P,J", out var commands, out var error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Equal(
            new[]
            {
                Command.Forward, Command.Forward, Command.Left, Command.Jump, Command.Forward,
                Command.Forward, Command.Crouch, Command.Back, Command.Jump
            },
            commands);
    }

    [Theory]
    [InlineData("n,p,r,l,j,i")]
    [InlineData("N P R L J I")]
    [InlineData("NPRLJİ")]
    [InlineData(" n , p , r , l , j , ı ")]
    public void Accepts_lowercase_spaces_and_turkish_dotless_variants(string input)
    {
        var ok = ScenarioParser.TryParse(input, out var commands, out _);

        Assert.True(ok);
        Assert.Equal(6, commands.Count);
        Assert.Equal(Command.Crouch, commands[^1]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(",,,")]
    [InlineData(null)]
    public void Rejects_empty_scenarios(string? input)
    {
        Assert.False(ScenarioParser.TryParse(input, out var commands, out var error));
        Assert.Empty(commands);
        Assert.NotNull(error);
    }

    [Fact]
    public void Reports_position_of_first_invalid_command()
    {
        var ok = ScenarioParser.TryParse("N,N,X,J", out var commands, out var error);

        Assert.False(ok);
        Assert.Empty(commands);
        Assert.Contains("3.", error);
        Assert.Contains("'X'", error);
    }
}
