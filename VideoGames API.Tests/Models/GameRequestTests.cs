using System.ComponentModel.DataAnnotations;
using VideoGames_API.Models;
using Xunit;

namespace VideoGames_API.Tests.Models;

public sealed class GameRequestTests
{
    [Fact]
    public void ValidGameRequest_PassesValidation()
    {
        var request = new GameRequest
        {
            Title = "Minecraft",
            ReleaseYear = 2011,
            Genre = "Sandbox"
        };

        Assert.Empty(Validate(request));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void MissingTitle_FailsValidation(string? title)
    {
        var request = new GameRequest { Title = title! };

        Assert.Contains(Validate(request), error => error.MemberNames.Contains(nameof(GameRequest.Title)));
    }

    [Fact]
    public void TitleLongerThan200Characters_FailsValidation()
    {
        var request = new GameRequest { Title = new string('A', 201) };

        Assert.Contains(Validate(request), error => error.MemberNames.Contains(nameof(GameRequest.Title)));
    }

    [Theory]
    [InlineData(999)]
    [InlineData(10000)]
    public void ReleaseYearThatIsNotFourDigits_FailsValidation(short releaseYear)
    {
        var request = new GameRequest { Title = "Minecraft", ReleaseYear = releaseYear };

        Assert.Contains(Validate(request), error => error.MemberNames.Contains(nameof(GameRequest.ReleaseYear)));
    }

    [Fact]
    public void GenreLongerThan100Characters_FailsValidation()
    {
        var request = new GameRequest
        {
            Title = "Minecraft",
            Genre = new string('A', 101)
        };

        Assert.Contains(Validate(request), error => error.MemberNames.Contains(nameof(GameRequest.Genre)));
    }

    private static IReadOnlyList<ValidationResult> Validate(GameRequest request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
        return results;
    }
}
