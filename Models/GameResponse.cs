namespace VideoGames_API.Models;

public sealed class GameResponse
{
    public int GameId { get; init; }
    public string Title { get; init; } = string.Empty;
    public short? ReleaseYear { get; init; }
    public string? Genre { get; init; }
}
