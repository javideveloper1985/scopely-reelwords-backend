using ReelWords.Domain.Entities;

namespace ReelWords.Domain.Repositories;

public interface IGameRepository
{
    Task<Game?> GetGameByUserId(string userId);
    Task<string> Create(Game game);
}