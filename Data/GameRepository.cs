using Dapper;
using Microsoft.Data.SqlClient;
using VideoGames_API.Models;

namespace VideoGames_API.Data;

public sealed class GameRepository(IConfiguration configuration) : IGameRepository
{
    private const string SelectColumns = "GameId, Title, ReleaseYear, Genre";
    private readonly string _connectionString = configuration.GetConnectionString("VideoGamesConnection")
        ?? throw new InvalidOperationException("The VideoGamesConnection connection string is not configured.");

    public async Task<IReadOnlyList<GameResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = $"SELECT {SelectColumns} FROM dbo.Games ORDER BY GameId;";

        await using var connection = new SqlConnection(_connectionString);
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var games = await connection.QueryAsync<GameResponse>(command);
        return games.AsList();
    }

    public async Task<GameResponse?> GetByIdAsync(int gameId, CancellationToken cancellationToken)
    {
        const string sql = $"SELECT {SelectColumns} FROM dbo.Games WHERE GameId = @GameId;";

        await using var connection = new SqlConnection(_connectionString);
        var command = new CommandDefinition(sql, new { GameId = gameId }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<GameResponse>(command);
    }

    public async Task<int> CreateAsync(GameRequest request, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO dbo.Games (Title, ReleaseYear, Genre)
            OUTPUT INSERTED.GameId
            VALUES (@Title, @ReleaseYear, @Genre);
            """;

        await using var connection = new SqlConnection(_connectionString);
        var command = new CommandDefinition(sql, request, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<bool> UpdateAsync(int gameId, GameRequest request, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE dbo.Games
            SET Title = @Title, ReleaseYear = @ReleaseYear, Genre = @Genre
            WHERE GameId = @GameId;
            """;

        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { GameId = gameId, request.Title, request.ReleaseYear, request.Genre };
        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        return await connection.ExecuteAsync(command) == 1;
    }

    public async Task<bool> DeleteAsync(int gameId, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM dbo.Games WHERE GameId = @GameId;";

        await using var connection = new SqlConnection(_connectionString);
        var command = new CommandDefinition(sql, new { GameId = gameId }, cancellationToken: cancellationToken);
        return await connection.ExecuteAsync(command) == 1;
    }
}
