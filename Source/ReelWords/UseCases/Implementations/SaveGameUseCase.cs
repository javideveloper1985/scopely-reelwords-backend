using ReelWords.Domain.Entities;
using ReelWords.Domain.Services;
using Scopely.Core.Result;
using System;
using System.Threading.Tasks;

namespace ReelWords.UseCases.Implementations;

public class SaveGameUseCase : ISaveGameUseCase
{
    private readonly ISaveGameService _saveGameService;

    public SaveGameUseCase(ISaveGameService gameRepository)
    {
        _saveGameService = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
    }

    public async Task<Result<string>> Execute(Game game)
    {
        try
        {
            var gameId = await _saveGameService.Save(game);
            return Result<string>.Ok(gameId);
        }
        catch (Exception ex)
        {
            return Result<string>.Unexpected(ex);
        }
    }
}