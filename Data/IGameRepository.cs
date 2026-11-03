using VideoGames_API.Models;

namespace VideoGames_API.Data;

public interface IGameRepository
{
    Task<IReadOnlyList<GameResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<GameResponse?> GetByIdAsync(int gameId, CancellationToken cancellationToken);
    Task<int> CreateAsync(GameRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(int gameId, GameRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int gameId, CancellationToken cancellationToken);
}
