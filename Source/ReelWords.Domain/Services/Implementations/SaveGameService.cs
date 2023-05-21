using ReelWords.Domain.Entities;
using ReelWords.Domain.Repositories;

namespace ReelWords.Domain.Services.Implementations;

public class SaveGameService : ISaveGameService
{
    private readonly IGameRepository _gameRepository;

    public SaveGameService(IGameRepository gameRepository) 
        => _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));

    public async Task<Game?> GetSavedGames(string userId)
        => await _gameRepository.GetGameByUserId(userId);

    public async Task<string> Save(Game game) 
        => await _gameRepository.Create(game);
}