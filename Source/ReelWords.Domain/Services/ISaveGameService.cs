using ReelWords.Domain.Entities;

namespace ReelWords.Domain.Services;

public interface ISaveGameService
{
    Task<Game?> GetSavedGames(string userId);
    Task<string> Save(Game game);
}